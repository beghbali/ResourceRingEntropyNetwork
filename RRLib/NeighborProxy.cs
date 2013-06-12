using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using RRLib;

namespace RRLib
{
	/// <summary>
	/// Summary description for NeighborProxy.
	/// </summary>
	public class NeighborProxy
	{
		public NeighborProxy()
		{
		}

		public void sendAndReceiveHello(Peer neighbor, Ring ring)
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
				
				RingInfo ringInfo = user.findRingInfo(ring.ringID);
				serializer.Serialize(stream, Constants.MessageTypes.MSG_HELLO);
				serializer.Serialize(stream, ring.ringID);
				serializer.Serialize(stream, ringInfo.token);
				serializer.Serialize(stream, new Peer(user.node, user.publicUserInfo, ringInfo.IE));
				
				reply = NetLib.communicate(neighbor.node.syncCommunicationPoint, message, true);
				
				Debug.Assert(reply != null, "Neighbor did not reply to Hello");

				BinaryFormatter deserializer = new BinaryFormatter();
				stream = new MemoryStream(reply);
				NetLib.bypassEntropyHeader(deserializer, stream);
				Constants.MessageTypes replyMsg = (Constants.MessageTypes)deserializer.Deserialize(stream);
				switch(replyMsg)
				{
					case Constants.MessageTypes.MSG_HELLO:
						uint ringID = (uint)deserializer.Deserialize(stream);
						if(ringID != ring.ringID)
							return;

						ulong token = (ulong)deserializer.Deserialize(stream);
						Peer peer = (Peer)deserializer.Deserialize(stream);

						IPEndPoint neighborIPEndPoint = peer.node.syncCommunicationPoint;
						if(!neighborIPEndPoint.Address.Equals(neighbor.node.syncCommunicationPoint.Address))
							//REVISIT: alert security system
							return;

						neighbor.node.syncCommunicationPoint = neighborIPEndPoint;
						neighbor.node.asyncCommunicationPoint = peer.node.asyncCommunicationPoint;
						neighbor.IE = peer.IE;
						break;
					case Constants.MessageTypes.MSG_DISCONNECT:
						neighbor.IE = null;
						break;
					default:
						neighbor.IE = null;
						break;
				}
			}
			catch (Exception e)
			{
				int x = 2;
			}
		}

		public void receiveAndSendHello(RingInfo ringInfo, Peer peer, Neighbor neighbor, 
										BinaryWriter writer)
		{
			if(!peer.node.syncCommunicationPoint.Address.Equals(neighbor.peer.node.syncCommunicationPoint.Address))
				//REVISIT: alert security system
				return;

			neighbor.peer.node.syncCommunicationPoint = peer.node.syncCommunicationPoint;
			neighbor.peer.node.asyncCommunicationPoint = peer.node.asyncCommunicationPoint;
			neighbor.peer.IE = peer.IE;
			
			byte[] message = new byte[Constants.WRITEBUFFSIZE];
			MemoryStream stream;

			try 
			{
				User user = User.getInstance();
				BinaryFormatter serializer = new BinaryFormatter();
				stream = new MemoryStream(message);
				
				NetLib.insertEntropyHeader(serializer, stream);

				serializer.Serialize(stream, Constants.MessageTypes.MSG_HELLO);
				serializer.Serialize(stream, ringInfo.ring.ringID);
				serializer.Serialize(stream, ringInfo.token);
				serializer.Serialize(stream, new Peer(user.node, user.publicUserInfo, ringInfo.IE));
				writer.Write(message);
				writer.Flush();
			}
			catch (Exception e)
			{
				int x = 2;
			}
		}

		public void sendQuery(Peer neighbor, RingInfo ringInfo, string[] columns, string[] query, 
			Node sender, int ttl)
		{
			byte[] message = new byte[Constants.WRITEBUFFSIZE];
			MemoryStream stream;

			if (ttl <= 0)
				return;

			try 
			{
				BinaryFormatter serializer = new BinaryFormatter();
				stream = new MemoryStream(message);
				
				NetLib.insertEntropyHeader(serializer, stream);

				if (columns == null)
					serializer.Serialize(stream, Constants.MessageTypes.MSG_SIMPLEQUERY);
				else
					serializer.Serialize(stream, Constants.MessageTypes.MSG_COLUMNQUERY);

				serializer.Serialize(stream, ringInfo.token);
				serializer.Serialize(stream, ringInfo.ring.ringID);
				serializer.Serialize(stream, sender.asyncCommunicationPoint);
				serializer.Serialize(stream, ttl);  //TTL
				if (columns == null)
					serializer.Serialize(stream, Constants.NULL_STRING_ARRAY);
				else
					serializer.Serialize(stream, columns);
				serializer.Serialize(stream, query);
				
				NetLib.communicateAsync(neighbor.node.asyncCommunicationPoint, message, false);
			}
			catch (Exception e)
			{
				int x = 2;  //REVISIT: probably need to send info to logger.
			}
		}
	}
}
