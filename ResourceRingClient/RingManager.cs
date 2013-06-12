using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using RRLib;

namespace ResourceRing
{
	/// <summary>
	/// Summary description for RingManager.
	/// </summary>
	public class RingManager
	{
		//REVISIT: This should probably be ringInfo and we can remove all the ringInfo searches on ID
		private ArrayList _rings;
		public ArrayList rings
		{
			get { return _rings; }
			set { _rings = value; }
		}

		public RingManager()
		{
			//start with Resource Ring
			rings = new ArrayList(Constants.DEFAULT_RINGS);
		}

		public void retreiveRingNeighbors()
		{
			foreach (Ring ring in rings)
			{
				foreach (IPEndPoint lord in ring.lords)
				{
					//retreive ring neighbors
					getRingNeighbors(lord, ring);
				}
			}
		}

		public void establishNeighborhoods()
		{
			RingInfo ringInfo;
			foreach (Ring ring in rings)
			{
				ringInfo = RingInfo.findRingInfoByID(User.getInstance().ringsInfo, ring.ringID);
				foreach (Neighbor neighbor in ringInfo.neighbors)
				{
					//See if the neighbor will accept you
					neighbor.neighborProxy.sendAndReceiveHello(neighbor.peer, ring);
				}
			}
		}

		private void getRingNeighbors(IPEndPoint lord, Ring ring)
		{
			byte[] message = new byte[Constants.WRITEBUFFSIZE];
			byte[] reply = new byte[Constants.READBUFFSIZE];
			MemoryStream stream;

			try 
			{
				User user = User.getInstance();
				BinaryFormatter serializer = new BinaryFormatter();
				stream = new MemoryStream(message);
				
				NetLib.insertEntropyHeader(serializer, stream);
				
				serializer.Serialize(stream, Constants.MessageTypes.MSG_RINGLOGIN);
				serializer.Serialize(stream, ring.ringID);
				RingInfo ringInfo = user.findRingInfo(ring.ringID);
				serializer.Serialize(stream, ringInfo.userName);
				serializer.Serialize(stream, SecurityLib.encrypt(ringInfo.password));
				serializer.Serialize(stream, user.node.syncCommunicationPoint);
				serializer.Serialize(stream, ringInfo.IE);

				reply = NetLib.communicate(lord, message, true);
				
				BinaryFormatter deserializer = new BinaryFormatter();
				stream = new MemoryStream(reply);
				NetLib.bypassEntropyHeader(deserializer, stream);
				Constants.MessageTypes replyMsg = (Constants.MessageTypes)deserializer.Deserialize(stream);
				Constants.QueryHandlerType howToHandleQuery;
				switch(replyMsg)
				{
					case Constants.MessageTypes.MSG_RINGNEIGHBORS:
						ringInfo.token = (ulong)serializer.Deserialize(stream);
						howToHandleQuery = (Constants.QueryHandlerType)serializer.Deserialize(stream);
						ringInfo.queryServer = (IPEndPoint)serializer.Deserialize(stream);
						switch(howToHandleQuery)
						{
							case Constants.QueryHandlerType.SERVER_FORM_QUERY:
								ringInfo.searchRing = 
									new RingInfo.ringSearcher(ringInfo.searchRingServerFormQuery);		
								break;
							case Constants.QueryHandlerType.SERVER_RESOLVE_QUERY:
								ringInfo.searchRing = 
									new RingInfo.ringSearcher(ringInfo.searchRingServerResolveQuery);
								break;
							case Constants.QueryHandlerType.DISTRIBUTED_PEER_QUERY:
								ringInfo.searchRing = 
									new RingInfo.ringSearcher(ringInfo.searchRingDistributedP2PEntropy);
								break;
						}
		
						Peer[] neighbors = (Peer[])deserializer.Deserialize(stream);
						foreach (Peer neighbor in neighbors)
							ringInfo.neighbors.Add(new Neighbor(neighbor, new NeighborProxy()));
						break;
					default:
						ringInfo.neighbors = null;
						break;
				}
			}
			catch (Exception e)
			{
				int x = 2;
			}
		}
	}
}
