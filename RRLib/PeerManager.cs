using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace RRLib
{
	/// <summary>
	/// Summary description for PeerManager.
	/// </summary>
	public class PeerManager
	{
		public class WeightedPeer : IComparable
		{
			private Peer _peer;
			public Peer peer
			{
				get { return _peer; }
				set { _peer = value; }
			}
		
			private float _weight;
			public float weight
			{
				get { return _weight; }
				set { _weight = value; }
			}

			public WeightedPeer(float __weight, Peer __peer)
			{
				weight = __weight;
				peer = __peer;
			}

			public int CompareTo(object other)
			{
				if (weight < ((WeightedPeer)other).weight)
					return -1;
			    if (weight > ((WeightedPeer)other).weight)
					return 1;
				
				return 0;
			}
		}

		private InvertedIndex contentIndex;

		public PeerManager()
		{
			contentIndex = new InvertedIndex();	
		}

		public void addPeer(Peer newPeer)
		{
			RRLib.SortedList peers;
		
			foreach (InformationEntropy IE in newPeer.IE)
			{
				peers = (RRLib.SortedList)contentIndex.getInvertedList(IE.keyword);
				if (peers == null)
				{
					peers = new RRLib.SortedList();
					contentIndex.add(IE.keyword, peers);
				}
				
				peers.Add(new WeightedPeer(IE.weight, newPeer));
			}
		}

		public Peer[] getAdjacentPeers(Peer peer, int IEConsiderationLength, int peersConsiderationLength, int maxPeers)
		{	
			int IEcount = 0, peerCount, matchIndex = 0;
			WeightedPeer[] matches = new WeightedPeer[IEConsiderationLength * peersConsiderationLength];
			RRLib.SortedList peers;
			IEnumerator peersWalker;
			Peer match;

			foreach (InformationEntropy IE in peer.IE)
			{
				if (++IEcount == IEConsiderationLength)
					break;
				peers = (RRLib.SortedList)contentIndex.getInvertedList(IE.keyword);
				if (peers == null)
					continue;
				peersWalker = peers.GetEnumerator();
				peerCount = 0;
				
				while(peersWalker.MoveNext() && peerCount++ < peersConsiderationLength) 
				{
					if(((WeightedPeer)peersWalker.Current).peer.token == peer.token)
						continue;
					matches[matchIndex++] = (WeightedPeer)peersWalker.Current;
				}
			}

			Array.Sort(matches, 0, matchIndex);
			
			Peer[] peerMatches = new Peer[matchIndex > maxPeers ? maxPeers : matchIndex];
			
			for (matchIndex = 0; matchIndex < peerMatches.Length; matchIndex++)
			{
				peerMatches[matchIndex] = matches[matchIndex].peer;
			}

			return peerMatches;
		}

		public static void sendResourceInfo(IPEndPoint destination, RingInfo ringInfo, Resource[] resources)
		{
			byte[] message = new byte[Constants.WRITEBUFFSIZE];
			MemoryStream stream;

			try 
			{
				User user = User.getInstance();
				BinaryFormatter serializer = new BinaryFormatter();
				stream = new MemoryStream(message);
				
				NetLib.insertEntropyHeader(serializer, stream);
				
				serializer.Serialize(stream, Constants.MessageTypes.MSG_QUERYHIT);
				serializer.Serialize(stream, ringInfo.ring.ringID);
				serializer.Serialize(stream, ringInfo.token);
				serializer.Serialize(stream, new Peer(user.node, user.publicUserInfo, ringInfo.IE));
				serializer.Serialize(stream, resources.Length);

				foreach (Resource resource in resources)
				{
					serializer.Serialize(stream, resource.header);
				}
				
				NetLib.communicateAsync(destination, message, false);		
			}
			catch (Exception e)
			{
				int x = 2;
			}
		}
	}
}
