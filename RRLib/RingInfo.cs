using System;
using System.Runtime.Serialization;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace RRLib
{
	/// <summary>
	/// Summary description for RingInfo.
	/// </summary>
	[Serializable]
	public class RingInfo : UserInfo, ISerializable
	{
		public delegate void ringSearcher(string[] columnsToSearch, string[] query);

		private string _userName;
		public string userName
		{
			get { return _userName; }
			set { _userName = value; }
		}
		
		private byte[] _password;
		public byte[] password
		{
			get { return _password; }
			set { _password = value; }
		}

		private Ring _ring;
		public Ring ring
		{
			get { return _ring; }
			set { _ring = value; }
		}

		private InformationEntropy[] _IE;
		public InformationEntropy[] IE
		{
			get { return _IE; }
			set { _IE = value; }
		}

		private ArrayList _neighbors;
		public ArrayList neighbors
		{
			get { return _neighbors; }
			set { _neighbors = value; }
		}

		private ulong _token;
		public ulong token
		{
			get { return _token; }
			set { _token = value; }
		}

		private ringSearcher _searchRing;
		public ringSearcher searchRing
		{
			get { return _searchRing; }
			set { _searchRing = value; }
		}

		private IPEndPoint _queryServer;
		public IPEndPoint queryServer
		{
			get { return _queryServer; }
			set { _queryServer = value; }
		}

		private Cataloger _cataloger;
		public Cataloger cataloger
		{
			get { return _cataloger; }
			set{ _cataloger = value; }
		}

		public RingInfo(Ring __ring, string __userName, string __password)
		{
			ring = __ring;
			userName = __userName;
			password = System.Text.Encoding.ASCII.GetBytes(__password);
			neighbors = new ArrayList(Constants.MIN_NUM_NEIGHBORS);
		}

		
		//Deserializer
		public RingInfo(SerializationInfo info, StreamingContext ctxt)
		{
			userName = info.GetString("userName");
			password = System.Text.Encoding.ASCII.GetBytes(info.GetString("password"));
			IE = (InformationEntropy[])info.GetValue("IE", typeof(InformationEntropy[]));
		}

		//Serializer
		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("userName", userName);
			info.AddValue("password", System.Text.Encoding.ASCII.GetString(password));
			info.AddValue("IE", IE);
		}

		public static Ring findRingByID(RingInfo[] ringInfos, uint ringID)
		{
			for(uint ringInfo = 0; ringInfo < ringInfos.Length; ringInfo++)
			{
				if(ringInfos[ringInfo].ring.ringID == ringID)
					return ringInfos[ringInfo].ring;
			}
			return null;
		}

		public static RingInfo findRingInfoByID(RingInfo[] ringInfos, uint ringID)
		{
			for(uint ringInfo = 0; ringInfo < ringInfos.Length; ringInfo++)
			{
				if(ringInfos[ringInfo].ring.ringID == ringID)
					return ringInfos[ringInfo];
			}
			return null;
		}

		public static Neighbor findNeighbor(ArrayList neighbors, ulong token)
		{
			foreach (Neighbor neighbor in neighbors)
			{
				if(neighbor.peer.token == token)
					return neighbor;
			}
			return null;
		}

		public void searchRingServerFormQuery(string[] columnsToSearch, string[] query)
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
				
				//Send request for a formed query
				serializer.Serialize(stream, Constants.MessageTypes.MSG_FORMQUERY);
				serializer.Serialize(stream, token);
				serializer.Serialize(stream, ring.ringID);
				serializer.Serialize(stream, user.node.syncCommunicationPoint);
			
				if (columnsToSearch == null)
					serializer.Serialize(stream, Constants.NULL_STRING_ARRAY);
				else
					serializer.Serialize(stream, columnsToSearch);
				serializer.Serialize(stream, query);

				reply = NetLib.communicate(this.queryServer, message, true);
				
				Debug.Assert(reply != null, "Query server did not respond");

				BinaryFormatter deserializer = new BinaryFormatter();
				stream = new MemoryStream(reply);
				NetLib.bypassEntropyHeader(deserializer, stream);
				Constants.MessageTypes replyMsg = (Constants.MessageTypes)deserializer.Deserialize(stream);
				if(replyMsg != Constants.MessageTypes.MSG_QUERYFORMED)
					return;
				string[] formedQuery = (string[])deserializer.Deserialize(stream);

				//distribute the query to peers.
				distributeQuery(columnsToSearch, formedQuery, User.getInstance().node, 
					Constants.MAX_HOPS);
			}
			catch (Exception e)
			{
				int x = 2;
			}
		}

		public void searchRingServerResolveQuery(string[] columnsToSearch, string[] query)
		{
		}
		
		public void searchRingDistributedP2PEntropy(string[] columnsToSearch, string[] query)
		{
		}

		public void distributeQuery(string[] columnsToSearch, string[] query, Node sender, int ttl)
		{
			//Determine best choice neighbors
			Neighbor[] bestChoiceNeighbors = determineBestChoiceNeighbors(query);
			//Send out query to each best choice neighbor
			foreach (Neighbor neighbor in bestChoiceNeighbors)
			{
				if (neighbor == null)
					break;;
				neighbor.neighborProxy.sendQuery(neighbor.peer, this, columnsToSearch, query, sender,
					ttl);
			}
		}

		/// <summary>
		/// Returns the list of best choices for neighbors to send out the query request to. 
		/// The list is not sorted. REVISIT: perhaps the list should be sorted
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		private Neighbor[] determineBestChoiceNeighbors(string[] query)
		{
			Neighbor[] bestChoiceNeighbors = new Neighbor[neighbors.Count];
			uint insertIndex = 0;

			RelevanceRanking ranker = RelevanceRanking.getInstance();
			float score = 0.0f;

			foreach (Neighbor neighbor in neighbors)
			{
				score = ranker.calculateScore(query, neighbor.peer.IE);
				if (score >= Constants.PEER_SELECTION_THRESHOLD)
					bestChoiceNeighbors[insertIndex++] = neighbor;
			}
			if (insertIndex == 0)  //all neighbors had a score of 0.0
			{
				foreach (Neighbor neighbor in neighbors)
				{
					if (neighbor == null || insertIndex >= Constants.MAX_PEERS_TO_QUERY)
						break;
					bestChoiceNeighbors[insertIndex++] = neighbor;
				}
			}
			return bestChoiceNeighbors;
		}

		public bool areWeAMatch(string[] query)
		{
			RelevanceRanking ranker = RelevanceRanking.getInstance();
			float score = ranker.calculateScore(query, this.IE);
			if (score >= Constants.QUERY_MATCH_THRESHOLD)
				return true;

			return false;
		}
	}
}
