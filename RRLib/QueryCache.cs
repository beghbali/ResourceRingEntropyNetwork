using System;

namespace RRLib
{
	/// <summary>
	/// Represents a cached/processed query. 
	/// </summary>
	public class QueryCache
	{
		#region public member variables
		private string[] _query;
		public string[] query
		{
			get { return _query; }
			set { _query = value; }
		}

		private IPEndPoint _sender;
		public IPEndPoint sender
		{
			get { return _sender; }
			set { _sender = value; }
		}

		private uint _ringID;
		public uint ringID
		{
			get { return _ringID; }
			set { ringID = value; }
		}

		private DateTime _timestamp;
		public DateTime timestamp
		{
			get { return _timestamp; }
			set { _timestamp = value; }
		}
		#endregion

		#region constructors
		public QueryCache(string[] __query, IPEndPoint __sender, uint __ringID, DateTime __timestamp)
		{
			query = __query;
			sender = __sender;
			ringID = __ringID;
			timestamp = __timestamp;
		}
		#endregion
	}
}
