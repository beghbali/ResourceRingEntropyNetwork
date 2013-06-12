using System;

namespace RRLib
{
	/// <summary>
	/// Summary description for Download.
	/// </summary>
	public class Download
	{
		private Peer _peer;
		public Peer peer
		{
			get { return _peer; }
			set { _peer = value; }
		}

		private RingInfo _ringInfo;
		public RingInfo ringInfo
		{
			get { return _ringInfo; }
			set { _ringInfo = value; }
		}

		private ResourceHeader _header;
		public ResourceHeader header
		{
			get { return _header; }
			set { _header = value; }
		}

		private byte[] _buffer;
		public byte[] buffer
		{
			get { return _buffer; }
			set { _buffer = value; }
		}

		private long _bytesRead;
		public long bytesRead
		{
			get { return _bytesRead; }
			set { _bytesRead = value; }
		}

		private UICallBack _UIHandler;
		public UICallBack UIHandler
		{
			get { return _UIHandler; }
			set { _UIHandler = value; }
		}

		public Download(Peer __peer, RingInfo __ringInfo, ResourceHeader __header, 
			UICallBack __UIHandler)
		{
			peer = __peer;
			ringInfo = __ringInfo;
			header = __header;
			//REVISIT: make sure there is enough room for meta data. Also if very large you want to split up into a list of
			//buffers or circular buffer where the buffer is written to temp file at milestones.
			buffer = new byte[header.size];
			bytesRead = 0;
			UIHandler = __UIHandler;
		}
	}
}
