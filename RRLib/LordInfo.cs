using System;
using System.Runtime.Serialization;

namespace RRLib
{
	/// <summary>
	/// Summary description for LordInfo.
	/// </summary>
	[Serializable]
	public class LordInfo : ISerializable
	{
		#region public member variables
		private uint _ringID;
		public uint ringID
		{
			get { return _ringID; }
			set { _ringID = value; }
		}

		private IPEndPoint[] _lords;
		public IPEndPoint[] lords 
		{
			get { return _lords; }
			set { _lords = value; }
		}
		#endregion

		#region constructors

		public LordInfo()
		{
		}

		public LordInfo(uint __ringID, IPEndPoint __lord)
		{
			ringID = __ringID;
			lords = new IPEndPoint[1];
			lords[0] = __lord;
		}

		public LordInfo(uint __ringID, IPEndPoint[] __lords)
		{
			ringID = __ringID;
			lords = __lords;
		}
		#endregion

		#region serialization methods
		//Deserializer
		public LordInfo(SerializationInfo info, StreamingContext ctxt)
		{
			ringID = (uint)info.GetValue("ringID", typeof(uint));
			lords = (IPEndPoint[])info.GetValue("lords", typeof(IPEndPoint[]));
		}

		//Serializer
		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("ringID", ringID);
			info.AddValue("lords", lords);
		}
		#endregion
	}
}
