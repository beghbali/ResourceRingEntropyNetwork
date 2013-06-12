using System;

namespace RRLib
{
	/// <summary>
	/// Summary description for WeightedPeer.
	/// </summary>
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
}
