using System;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using RRLib;

namespace ResourceRing
{
	/// <summary>
	/// Summary description for Client.
	/// </summary>
	public class Client
	{
		#region private member variables
		private ServerProxy serverProxy;
		private uint numLoginRetries;
		private RingManager ringManager;
		private QueryProcessor queryProcessor;
		private RelevanceRanking relevanceRanker;
		private Thread entropyRouter, controlPlane;
		private DownloadManager downloadManager;
		private Hashtable queryCache;
		#endregion

		#region UI callback table
		//for calling back to the user interface (a delegate table)
		ClientUserInterface.callbackTable_ clientUserInterfaceCallbackTable;
		#endregion

		#region constructors
		public Client(ClientUserInterface.callbackTable_ _clientUserInterfaceCallbackTable)
		{
			clientUserInterfaceCallbackTable = _clientUserInterfaceCallbackTable;
			numLoginRetries = 0;
			serverProxy  = new ServerProxy();
			ringManager = new RingManager();
			queryProcessor = QueryProcessor.getInstance();
			relevanceRanker = RelevanceRanking.getInstance();
			downloadManager = new DownloadManager();
			queryCache = new Hashtable();
		}
		#endregion

		public void exitHandler(object sender, System.EventArgs e)
		{
			//log the user off from the network
			serverProxy.logoff(User.getInstance());
			if (entropyRouter != null)
				entropyRouter.Abort();
			entropyRouter = null;

			if (controlPlane != null)
				controlPlane.Abort();
			controlPlane = null;
		}

		public void submitSettings(string userName, string prefix, string firstName, string middleName,
			string lastName, string suffix, string streetAddr, string city, uint zipCode, string state,
			string country, byte[] password)
		{
			User user = User.getInstance();
			PublicUserInfo publicUserInfo = new PublicUserInfo();
			PrivateUserInfo privateUserInfo = new PrivateUserInfo(user.privateUserInfo.userID, prefix, 
				firstName, middleName, lastName, suffix, streetAddr, city, zipCode, state, country);

			user.changeInfo(publicUserInfo, privateUserInfo);
			//sync the changes to the server
			serverProxy.syncUserInfo(user);
		}

		public Constants.LoginStatus login(User user)
		{
			Constants.LoginStatus retval = Constants.LoginStatus.STATUS_SERVERNOTREACHED;
			
			while(!user.loggedIn&& numLoginRetries < Constants.MAXLOGINRETRIES)
			{
				retval = serverProxy.logon(user);
				switch(retval)
				{
					case Constants.LoginStatus.STATUS_SERVERNOTREACHED:
						//sleep and retry logging in
						Thread.Sleep(Constants.LOGINRETRYINTERVAL);
						break;
					default:
						break;
				}
				numLoginRetries++;
			}
			return retval;
		}

		private void signup(User user)
		{
			serverProxy.signup(user);
		}

		/// <summary>
		/// Runs in the background and cleans the query cache on an interval to prevent a client 
		/// sending the same search query multiple times within a certain period.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void QueryCacheInvalidator(object sender, System.Timers.ElapsedEventArgs e)
		{
			IDictionaryEnumerator enumerator = queryCache.GetEnumerator();
			ArrayList cacheList;
			while (enumerator.MoveNext())
			{
				cacheList = (ArrayList)enumerator.Value;
				foreach (QueryCache cachedQuery in cacheList)
				{
					if (cachedQuery.timestamp.Subtract(DateTime.Now).Ticks >= Constants.QUERYCACHE_INVALIDATE_PERIOD)
					{
						cacheList.Remove(cachedQuery);
						if (cacheList.Count == 0)
							queryCache.Remove(enumerator.Key);
					}
				}	
			}
		}

		/// <summary>
		/// Processes the search query from the peer
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="writer"></param>
		private void processPeerSearchQuery(BinaryReader reader, BinaryWriter writer)
		{
			BinaryFormatter deserializer = new BinaryFormatter();
			ulong token = (ulong)deserializer.Deserialize(reader.BaseStream);
			uint ringID = (uint)deserializer.Deserialize(reader.BaseStream);
			IPEndPoint senderCommPoint = (IPEndPoint)deserializer.Deserialize(reader.BaseStream);
			int TTL = (int)deserializer.Deserialize(reader.BaseStream);
			string[] columns = (string[])deserializer.Deserialize(reader.BaseStream);
			string[] query = (string[])deserializer.Deserialize(reader.BaseStream);
			
			RingInfo ringInfo = RingInfo.findRingInfoByID(User.getInstance().ringsInfo, ringID);
			Resource[] matchingResources;
			
			ArrayList matches = (ArrayList)queryCache[senderCommPoint];
			
			//Check to make sure someone is not resending the same query often
			if (matches != null)
			{
				foreach (QueryCache cachedQuery in matches)
				{
					if (LibUtil.QueriesEqual(cachedQuery.query, query) && ringID == cachedQuery.ringID)
						return;
				}
			}
			if (ringInfo.areWeAMatch(query))
			{
				matchingResources = relevanceRanker.rankDatabase(ringInfo.cataloger.resourcesIndex, query, columns);
				PeerManager.sendResourceInfo(senderCommPoint, ringInfo, matchingResources);
			}
			
			ringInfo.distributeQuery(columns, query, new Node(null, senderCommPoint, 
				Constants.LineSpeedType.LNSPEED_UNKNOWN), TTL-1);

			if (matches == null) 
				matches = new ArrayList();

			matches.Add(new QueryCache(query, senderCommPoint, ringID, DateTime.Now));
			queryCache.Add(senderCommPoint, matches);
		}

		private void processQueryHit(BinaryReader reader, BinaryWriter writer)
		{
			BinaryFormatter deserializer = new BinaryFormatter();
			uint ringID = (uint)deserializer.Deserialize(reader.BaseStream);
			ulong token = (ulong)deserializer.Deserialize(reader.BaseStream);
			Peer peerWithHit = (Peer)deserializer.Deserialize(reader.BaseStream);
			int numHits = (int)deserializer.Deserialize(reader.BaseStream);
			ResourceHeader resourceHeader;
			
			RingInfo ringInfo = RingInfo.findRingInfoByID(User.getInstance().ringsInfo, ringID);
			object[] callbackArgs = new Object[3];
			callbackArgs[0] = ringInfo;
			callbackArgs[1] = peerWithHit;

			while (numHits-- > 0)
			{
				resourceHeader = (ResourceHeader)deserializer.Deserialize(reader.BaseStream);
				callbackArgs[2] = resourceHeader;
				ClientUserInterface.clientUserInterfaceInstance.BeginInvoke(
					clientUserInterfaceCallbackTable.showQueryHit, callbackArgs);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="writer"></param>
		private void processHelloRequest(BinaryReader reader, BinaryWriter writer)
		{
			try
			{
				BinaryFormatter deserializer = new BinaryFormatter();
				uint ringID = (uint)deserializer.Deserialize(reader.BaseStream);
				ulong token = (ulong)deserializer.Deserialize(reader.BaseStream);
				Peer peer = (Peer)deserializer.Deserialize(reader.BaseStream);
				
				RingInfo ringInfo = RingInfo.findRingInfoByID(User.getInstance().ringsInfo, ringID);
				Neighbor neighbor = RingInfo.findNeighbor(ringInfo.neighbors, token);		
				
				if(neighbor == null)
				{
					neighbor = new Neighbor(peer, new NeighborProxy());
					ringInfo.neighbors.Add(neighbor);
				}
				neighbor.neighborProxy.receiveAndSendHello(ringInfo, peer, neighbor, writer);
			}
			catch (Exception e)
			{
				int x = 2;
			}
		}

		/// <summary>
		/// Will process all in bound communications from server or other peers
		/// </summary>
		/// <param name="reader">handle to read from the requester</param>
		/// <param name="writer">handle to write to the requester</param>
		private void processInboundRequests(BinaryReader reader, BinaryWriter writer)
		{
			BinaryFormatter deserializer = new BinaryFormatter();
			NetLib.bypassEntropyHeader(deserializer, reader.BaseStream);
			Constants.MessageTypes messageCode = 
				(Constants.MessageTypes)deserializer.Deserialize(reader.BaseStream);

			switch(messageCode)
			{
				case Constants.MessageTypes.MSG_SIMPLEQUERY:
				case Constants.MessageTypes.MSG_COLUMNQUERY:
					processPeerSearchQuery(reader, writer);
					break;
				case Constants.MessageTypes.MSG_QUERYHIT:
					processQueryHit(reader, writer);
					break;
				default:
					break;
			}
		}

		private void processControlPlaneRequests(BinaryReader reader, BinaryWriter writer)
		{
			BinaryFormatter deserializer = new BinaryFormatter();
			NetLib.bypassEntropyHeader(deserializer, reader.BaseStream);
			Constants.MessageTypes messageCode = 
				(Constants.MessageTypes)deserializer.Deserialize(reader.BaseStream);

			switch(messageCode)
			{
				case Constants.MessageTypes.MSG_HELLO:
					processHelloRequest(reader, writer);
					break;
				case Constants.MessageTypes.MSG_DOWNLOADREQUEST:
					downloadManager.HandleDownloadRequest(reader, writer);
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// This is the main client loop that will process the following:
		/// 1. client side requests for contacting server
		/// 2. client side requests for contacting other peers (e.g. search)
		/// 3. peer requests for processing
		/// 4. server requests for processing
		/// </summary>
		private void routeEntropyMessages()
		{
			RRLib.handleConnection processIncoming = new RRLib.handleConnection(processInboundRequests);
			User user = User.getInstance();
			IPEndPoint listenPort = user.node.asyncCommunicationPoint;

			NetLib.listenAndCommunicateAsync(listenPort, processIncoming);
		}

		private void processControlPlaneMessages()
		{
			RRLib.handleConnection processControlPlane = new RRLib.handleConnection(processControlPlaneRequests);
			User user = User.getInstance();
			IPEndPoint listenPort = user.node.syncCommunicationPoint;

			//long live the loop
			while(true)
			{
				NetLib.listenAndCommunicate(listenPort, processControlPlane);
			}
		}

		/// <summary>
		/// Searches for the particular query on the P2P network
		/// </summary>
		public void search(int[] ringIDs, string[] columns, string queryString)
		{
			RingInfo[] ringsInfo = User.getInstance().ringsInfo;
			string[] query = queryProcessor.queryFromQueryString(queryString);

			RingInfo ringInfo;
			for(uint index = 0; index < ringIDs.Length && ringIDs[index] != -1; index++)
			{
				ringInfo = RingInfo.findRingInfoByID(ringsInfo, (uint)ringIDs[index]);
				ringInfo.searchRing(columns, query);
			}
		}

		public void RetreiveResource (Peer peer, RingInfo ringInfo, ResourceHeader resourceHeader, 
			UICallBack UIHndlr)
		{
			downloadManager.Download(peer, ringInfo, resourceHeader, UIHndlr);
		}

		private void setupRings()
		{
			//*contact the lords and get PR neighbors
			//*insert PR neighbors for each PR
			ringManager.retreiveRingNeighbors();
			//*send hello messages to each PR neighbor and see what they say
			//*based on the replies from the PR neighbors, either contact the server again
			//to get additional neighbors or enable those PRs 
			ringManager.establishNeighborhoods();
		}
		
		private void setupCataloger()
		{
			ArrayList rings = ringManager.rings;

			//setup callback routines for the cataloger progress bar
			setupUIforCatalog UIsetupHandler = new setupUIforCatalog(
				ClientUserInterface.UI.setupCatalogUIHandler);
			updateUIforCatalog UIupdateHandler = new updateUIforCatalog(
				ClientUserInterface.UI.updateCatalogUIHandler);

			foreach (Ring ring in rings)
			{
				RingInfo ringInfo = RingInfo.findRingInfoByID(User.getInstance().ringsInfo, ring.ringID);
				ringInfo.cataloger = new Cataloger(UIsetupHandler, UIupdateHandler);
				
				//REVISIT: need to read configuration file to apply correct resource groups to each ring
				ringInfo.cataloger.groups.AddRange(ring.group);
				
				ringInfo.cataloger.roots.Add(new DirectoryInfo(@"C:\Documents and Settings\beghbali\My Documents\General\Friends"));
				//ringInfo.cataloger.roots.Add(new DirectoryInfo(@"C:\Documents and Settings\beghbali\My Documents\My Music"));
				ringInfo.cataloger.roots.Add(new DirectoryInfo(@"C:\Documents and Settings\beghbali\Desktop\Downloads\Software Downloads"));
				
				//REVISIT: do catalog leaking, namely check to see if any of the roots of rings are subset of another, You 
				//probably wnat one cataloger that walks the list of roots and each entry has a list of rings that need the
				//resources from that root (you need to calculate this as subsets) and will update the corresponding rings'
				//catalogers. This should be done via chain delegates (LIST registries).
				ringInfo.cataloger.catalog();
				
				//pick up the calculated IE
				ringInfo.IE = ringInfo.cataloger.calculatedIE;

				object[] callbackArgs = new Object[1];
				callbackArgs[0] = ringInfo;
				ClientUserInterface.clientUserInterfaceInstance.Invoke(
					clientUserInterfaceCallbackTable.showCatalog, callbackArgs);
			}
		}

		/// <summary>
		/// Start the client 
		/// </summary>
		public void start()
		{
			User user = User.getInstance();
			switch(login(user))
			{
				case Constants.LoginStatus.STATUS_LOGGEDIN:
				//invoke the user interface call back to update everything that depends on the user 
				//getting logged in (Asynchronous call to the UI thread)
					ClientUserInterface.clientUserInterfaceInstance.BeginInvoke(
								clientUserInterfaceCallbackTable.showPostLogin);
					break;
				case Constants.LoginStatus.STATUS_NOTAMEMBER:
					signup(user);
					break;
				default:
					break;
			}
			
			setupRings();

			controlPlane = new Thread(new ThreadStart(processControlPlaneMessages));
			controlPlane.Name = "Control-Plane";
			controlPlane.IsBackground = true;
			
			entropyRouter = new Thread(new ThreadStart(routeEntropyMessages));
			entropyRouter.Name = "Entropy Router";
			entropyRouter.IsBackground = true;
			
			System.Timers.Timer backgroundQueryCacheInvalidator = 
				new System.Timers.Timer(Constants.QUERYCACHE_INVALIDATE_PERIOD);
			backgroundQueryCacheInvalidator.Elapsed += 
				new System.Timers.ElapsedEventHandler(QueryCacheInvalidator);
			backgroundQueryCacheInvalidator.Enabled = true;


			//start the thread for Entropy protocol routing
			entropyRouter.Start();

			//start the thread for control-plane management
			controlPlane.Start();
			
			setupCataloger();
		}
	}
}
