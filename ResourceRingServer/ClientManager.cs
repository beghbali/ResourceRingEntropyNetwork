using System;
using System.Collections;
using RRLib;

namespace ResourceRingServer
{
	/// <summary>
	/// Manages peers
	/// </summary>
	public class ClientManager : IComparer
	{
		private System.Collections.SortedList clientDB;
		private System.Collections.SortedList sessionTable;

		public ClientManager()
		{
			clientDB = new System.Collections.SortedList();
			sessionTable = new System.Collections.SortedList();
		}

		public int Compare(object client1, object client2)
		{
			if(((ClientSession)client1).user.privateUserInfo.userID < 
				((ClientSession)client2).user.privateUserInfo.userID)
				return 0;
			else if(((ClientSession)client1).user.privateUserInfo.userID > 
				((ClientSession)client2).user.privateUserInfo.userID)
				return 1;
	
			return 0;								
		}

		public void addClient(Member client, ulong sessionID)
		{
			ClientSession newSession = new ClientSession(client, sessionID);
			clientDB.Add(newSession.sessionID, newSession);
			ArrayList clientSessions = new ArrayList(1);
			clientSessions.Add(newSession.sessionID);
			sessionTable.Add(client.privateUserInfo.userID, clientSessions);
		}

		public ClientSession findClientSession(Member client)
		{
			object clientSessionIDsObj = sessionTable[client.privateUserInfo.userID];
			if(clientSessionIDsObj == null)
				return null;

			ArrayList clientSessionIDs = (ArrayList)clientSessionIDsObj;
			for(int clientSessionIDIndx = 0; clientSessionIDIndx < clientSessionIDs.Count; 
				clientSessionIDIndx++)
			{
				ulong sessionID = (ulong)clientSessionIDs[clientSessionIDIndx];
				ClientSession foundClient = (ClientSession)clientDB[sessionID];
			
				//only one instance from a device per user is allowed
				if(foundClient.user.node.syncCommunicationPoint.Equals(client.node.syncCommunicationPoint))
					return foundClient;
			}

			return null;
		}

		public bool removeClient(Member client, ulong sessionID)
		{
			if(client == null)
				return false;

			object clientSessionObj = clientDB[sessionID];
			if(clientSessionObj == null)
				return false;

			clientDB.Remove(((ClientSession)clientSessionObj).sessionID);
			sessionTable.Remove(client.privateUserInfo.userID);
			return true;
		}

		public bool updateClient(Member client, ulong sessionID)
		{
			if(client == null)
				return false;
			
			object clientSessionObj = clientDB[sessionID];
			if(clientSessionObj == null)
				return false;

			ClientSession clientSession = (ClientSession)clientSessionObj;
			ClientSession newSession = new ClientSession(client, clientSession.sessionID);
			clientDB[clientSession.sessionID] = newSession;
			return true;
		}
	}
}
