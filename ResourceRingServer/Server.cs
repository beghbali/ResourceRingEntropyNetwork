using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

using RRLib;

namespace ResourceRingServer
{
	/// <summary>
	/// Summary description for Server.
	/// </summary>
	public class Server
	{
		//Class variables
		private ClientManager clientManager;
		private AccountsManager accountsManager;
		private ServerRingInfo[] serverRingsInfo;
		private ulong nextTokenID;

		//for calling back to the user interface (a delegate table)
		ServerUserInterface.callbackTable_ serverUserInterfaceCallbackTable;

		public Server(ServerUserInterface.callbackTable_ _serverUserInterfaceCallbackTable)
		{
			serverUserInterfaceCallbackTable = _serverUserInterfaceCallbackTable;
			clientManager = new ClientManager();
			accountsManager = new AccountsManager(@"accounts.xml");
			accountsManager.loadAccounts();

			//REVISIT: some hard-coded stuff here needs cleanup when file operations are in place
			serverRingsInfo = new ServerRingInfo[Constants.NUM_RINGS];
			AccountsManager ringAccountsManager;
			for(int index = 0; index < serverRingsInfo.Length; index ++)
			{
				ringAccountsManager = new AccountsManager(@"accounts.xml");
				ringAccountsManager.loadAccounts();
				serverRingsInfo[index] = new ServerRingInfo(Constants.DEFAULT_RINGS[index], 
					new ClientManager(), ringAccountsManager, new PeerManager(), 
					new ServerPolicyManager());
			}

			nextTokenID = 1;
		}

		public void start()
		{
			serverInternal();
		}

		private void serverInternal()
		{
			RRLib.handleConnection clientProcessor = new RRLib.handleConnection(process);
			NetLib.listenAndCommunicate(Constants.SERVER,  clientProcessor);
		}

		public void process(BinaryReader clientReader, BinaryWriter clientWriter)
		{
			BinaryFormatter deserializer = new BinaryFormatter();
			NetLib.bypassEntropyHeader(deserializer, clientReader.BaseStream);
			Constants.MessageTypes messageCode = (Constants.MessageTypes)deserializer.Deserialize(clientReader.BaseStream);

			switch(messageCode)
			{
				case Constants.MessageTypes.MSG_LOGIN:
					processClientLogin(clientReader, clientWriter);
					break;
				case Constants.MessageTypes.MSG_RINGLOGIN:
					processClientRingLogin(clientReader, clientWriter);
					break;
				case Constants.MessageTypes.MSG_LOGOFF:
					processClientLogoff(clientReader, clientWriter);
					break;
				case Constants.MessageTypes.MSG_SIGNUP:
					break;
				case Constants.MessageTypes.MSG_SYNCUSRINFO:
					processClientSyncUserInfo(clientReader, clientWriter);
					break;
				case Constants.MessageTypes.MSG_FORMQUERY:
					formQueryForClient(clientReader, clientWriter);
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Login a client to the server and dispatch it membership info as well as adjacent neighbors
		/// </summary>
		/// <param name="clientReader">A handle for reading client Login message</param>
		/// <param name="clientWriter">A handle for writing to the client</param>
		private void processClientLogin(BinaryReader clientReader, BinaryWriter clientWriter)
		{
			//deserialize the message
			BinaryFormatter deserializer = new BinaryFormatter();
			uint ringID = (uint)deserializer.Deserialize(clientReader.BaseStream);
			string userName = (string)deserializer.Deserialize(clientReader.BaseStream);
			byte[] password = (byte[])deserializer.Deserialize(clientReader.BaseStream);
			IPEndPoint syncCommPoint = (IPEndPoint)deserializer.Deserialize(clientReader.BaseStream);

			byte[] message = new byte[Constants.WRITEBUFFSIZE];
			MemoryStream stream = new MemoryStream(message);
			BinaryFormatter serializer = deserializer; //reuse the object
			Member member = null;

			NetLib.insertEntropyHeader(serializer, stream);
			//not a member need to signup first, goodbye =)
			if((member = accountsManager.findMemberByUserName(userName)) == null)
			{
				deserializer.Serialize(stream, Constants.MessageTypes.MSG_NOTAMEMBER);
				clientWriter.Write(message);
				clientWriter.Flush();
				return;
			}

			//Already on the network from the requesting device (are you a hacker?)
			if(clientManager.findClientSession(member) != null)
			{
				deserializer.Serialize(stream, Constants.MessageTypes.MSG_ALREADYSIGNEDIN);
				clientWriter.Write(message);
				clientWriter.Flush();
				return;
			}

			//get a session ID add the user to the client manager database
			ulong tokenID = getNextTokenID();
			
			//get users current node settings
			member.node.syncCommunicationPoint = syncCommPoint;
			clientManager.addClient(member, tokenID);

			//give the session id and adjacent peers
			serializer.Serialize(stream, Constants.MessageTypes.MSG_OK);
			serializer.Serialize(stream, tokenID);
			serializer.Serialize(stream, (uint)member.lords.Length);
			foreach (LordInfo lord in member.lords)
			{
				serializer.Serialize(stream, lord);
			}
	
			clientWriter.Write(message);
			clientWriter.Flush();

			//invoke the user interface call back to update everything that depends on a new user 
			//joining the network
			Member[] methodInvokeArgs = new Member[1];
			methodInvokeArgs[0] = member;
			//Asynchronous call to the UI thread
			ServerUserInterface.serverUserInterfaceInstance.BeginInvoke(
				serverUserInterfaceCallbackTable.addNewClient, methodInvokeArgs);
		}

		private void processClientRingLogin(BinaryReader clientReader, BinaryWriter clientWriter)
		{
			//deserialize the message
			BinaryFormatter deserializer = new BinaryFormatter();
			uint ringID = (uint)deserializer.Deserialize(clientReader.BaseStream);
			string userName = (string)deserializer.Deserialize(clientReader.BaseStream);
			byte[] password = (byte[])deserializer.Deserialize(clientReader.BaseStream);
			IPEndPoint commPoint = (IPEndPoint)deserializer.Deserialize(clientReader.BaseStream);
			InformationEntropy[] IE = (InformationEntropy[])deserializer.Deserialize(clientReader.BaseStream);

			ServerRingInfo serverRingInfo = ServerRingInfo.findServerRingInfoByID(this.serverRingsInfo, ringID);
			byte[] message = new byte[Constants.WRITEBUFFSIZE];
			MemoryStream stream = new MemoryStream(message);
			BinaryFormatter serializer = deserializer; //reuse the object
			Member member = null;

			NetLib.insertEntropyHeader(serializer, stream);

			//not a member need to signup first, goodbye =)
			if((member = accountsManager.findMemberByUserName(userName)) == null)
			{
				deserializer.Serialize(stream, Constants.MessageTypes.MSG_NOTAMEMBER);
				clientWriter.Write(message);
				clientWriter.Flush();
				return;
			}

			ClientSession session;
			ulong sessionID;
			Peer memberAsPeer = new Peer(member.node, member.publicUserInfo, IE);
			
			//get users current node settings
			member.node.syncCommunicationPoint = commPoint;

			//Already on the network from the requesting device (are you a hacker?)
			if((session = serverRingInfo.ringClientManager.findClientSession(member)) == null)
			{
				//get a session ID add the user to the client manager database
				sessionID = serverRingInfo.getNextSessionID();
				
				serverRingInfo.ringClientManager.addClient(member, sessionID);
			}
			else
				sessionID = session.sessionID;

			memberAsPeer.token = sessionID;
			//give the session id and adjacent peers
			serializer.Serialize(stream, Constants.MessageTypes.MSG_RINGNEIGHBORS);
			serializer.Serialize(stream, sessionID);
			serializer.Serialize(stream, serverRingInfo.serverPolicyManager.getSearchQueryPolicy());
			serializer.Serialize(stream, serverRingInfo.serverPolicyManager.queryServer);
			serializer.Serialize(stream, serverRingInfo.ringPeerManager.getAdjacentPeers(memberAsPeer,
				Constants.IE_MAX_KEYWORDS_TO_CONSIDER, Constants.MAX_PEERS_TO_CONSIDER_FOR_KEYWORD,
				Constants.MAX_PEERS));
	
			clientWriter.Write(message);
			clientWriter.Flush();

			//Now add the new peer to the database
			serverRingInfo.ringPeerManager.addPeer(memberAsPeer);

			//invoke the user interface call back to update everything that depends on a new user 
			//joining the network
			Member[] methodInvokeArgs = new Member[1];
			methodInvokeArgs[0] = member;
			//Asynchronous call to the UI thread
			ServerUserInterface.serverUserInterfaceInstance.BeginInvoke(
				serverUserInterfaceCallbackTable.addNewClient, methodInvokeArgs);
		}

		/// <summary>
		/// Logoff a client and release resources
		/// </summary>
		/// <param name="clientReader">A handle for reading client Logoff message</param>
		/// <param name="clientWriter">A handle for writing to the client</param>
		private void processClientLogoff(BinaryReader clientReader, BinaryWriter clientWriter)
		{
			//deserialize the message
			BinaryFormatter deserializer = new BinaryFormatter();
			ulong sessionID = (ulong)deserializer.Deserialize(clientReader.BaseStream);
			uint ringID = (uint)deserializer.Deserialize(clientReader.BaseStream);
			string userName = (string)deserializer.Deserialize(clientReader.BaseStream);
			byte[] password = (byte[])deserializer.Deserialize(clientReader.BaseStream);
		
			Member member;
			//make sure that the user is the member it claims to be
			if((member = accountsManager.findMemberByUserName(userName)) == null)
				return;

			//remove the user from the client manager database
			if(!clientManager.removeClient(member, sessionID))
				return;
			
			//invoke the user interface call back to update everything that depends on an existing user 
			//leaving the network
			Member[] methodInvokeArgs = new Member[1];
			methodInvokeArgs[0] = member;
			//Asynchronous call to the UI thread
			ServerUserInterface.serverUserInterfaceInstance.BeginInvoke(serverUserInterfaceCallbackTable.removeExistingClient, methodInvokeArgs);
		}

		private void processClientSyncUserInfo(BinaryReader clientReader, BinaryWriter clientWriter)
		{
			//deserialize the message
			BinaryFormatter deserializer = new BinaryFormatter();
			ulong sessionID = (ulong)deserializer.Deserialize(clientReader.BaseStream);
			uint ringID = (uint)deserializer.Deserialize(clientReader.BaseStream);
			string userName = (string)deserializer.Deserialize(clientReader.BaseStream);
			byte[] password = (byte[])deserializer.Deserialize(clientReader.BaseStream);
			Member member = (Member)deserializer.Deserialize(clientReader.BaseStream);
			
			Member existingMember;
			//make sure that the user is the member it claims to be
			if((existingMember = accountsManager.findMemberByUserName(userName)) == null)
				return;

			//update client manager with the new info
			if(!clientManager.updateClient(existingMember, sessionID))
				return;
			
			//update accounts manager with the user's new info
			accountsManager.updateAccount(userName, password, member);

			//invoke the user interface call back to update everything that depends on an existing user
			//updating its settings
			Member[] methodInvokeArgs = new Member[1];
			methodInvokeArgs[0] = existingMember;
			//Asynchronous call to the UI thread
			ServerUserInterface.serverUserInterfaceInstance.BeginInvoke(serverUserInterfaceCallbackTable.updateExistingClient, methodInvokeArgs);
		}

		private void formQueryForClient(BinaryReader clientReader, BinaryWriter clientWriter)
		{
			try
			{
				//deserialize the message
				BinaryFormatter deserializer = new BinaryFormatter();
				ulong sessionID = (ulong)deserializer.Deserialize(clientReader.BaseStream);
				uint ringID = (uint)deserializer.Deserialize(clientReader.BaseStream);
				IPEndPoint commPoint = (IPEndPoint)deserializer.Deserialize(clientReader.BaseStream);
				string[] columns = (string[])deserializer.Deserialize(clientReader.BaseStream);
				string[] query = (string[])deserializer.Deserialize(clientReader.BaseStream);

				//REVISIT: query database and return valid tokens for the search

				byte[] message = new byte[Constants.WRITEBUFFSIZE];
				MemoryStream stream = new MemoryStream(message);
				BinaryFormatter serializer = deserializer; //reuse the object

				NetLib.insertEntropyHeader(serializer, stream);

				serializer.Serialize(stream, Constants.MessageTypes.MSG_QUERYFORMED);
				serializer.Serialize(stream, query);

				clientWriter.Write(message);
				clientWriter.Flush();
			}
			catch (Exception e)
			{
				MessageBox.Show("something failed");
			}
		}

		public void exitHandler(object sender, System.EventArgs e)
		{
			//Do any cleanup necessary when server exits.
		}

		public ulong getNextTokenID()
		{
			return nextTokenID++;
		}
	}
}
