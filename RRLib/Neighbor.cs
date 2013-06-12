using System;

namespace RRLib
{
	/// <summary>
	/// Summary description for Neighbor.
	/// </summary>
	public class Neighbor
	{
		private Peer _peer;
		public Peer peer
		{
			get { return _peer; }
			set { _peer = value; }
		}

		private NeighborProxy _neighborProxy;
		public NeighborProxy neighborProxy
		{
			get { return _neighborProxy; }
			set { _neighborProxy = value; }
		}

		public Neighbor(Peer __peer, NeighborProxy __neighborProxy)
		{
			peer = __peer;
			neighborProxy = __neighborProxy;
		}
	}
}
