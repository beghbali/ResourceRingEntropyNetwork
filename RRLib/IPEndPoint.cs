using System;
using System.Runtime.Serialization;

namespace RRLib
{
	/// <summary>
	/// An XmlSerializer friendly IPEndPoint class. Has a default parameterless constructor
	/// </summary>
	[Serializable]
	public class IPEndPoint : ISerializable
	{
		#region public member variables
		private long _Address;
		public long Address
		{
			get { return _Address; }
			set { _Address = value; }
		}

		private int _Port;
		public int Port
		{
			get { return _Port; }
			set { _Port = value; }
		}
		#endregion

		#region constructors
		public IPEndPoint()
		{
		}

		public IPEndPoint(long __Address, int __Port)
		{
			Address = __Address;
			Port = __Port;
		}

		public IPEndPoint(System.Net.IPAddress __Address, int __Port)
		{
			Address = __Address.Address;
			Port = __Port;
		}
		#endregion

		#region properties
		public System.Net.IPEndPoint GetEndPoint()
		{
			return new System.Net.IPEndPoint(Address, Port); 
		}
		#endregion

		#region serialization methods
		//Deserializer
		public IPEndPoint(SerializationInfo info, StreamingContext ctxt)
		{
			Address = info.GetInt64("Address");
			Port = info.GetInt32("Port");
		}
		
		//Serializer
		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("Address", Address);
			info.AddValue("Port", Port);
		}
		#endregion

		#region Equals method
		public bool Equals(object other)
		{
			if (!(other is IPEndPoint))
				throw new ArgumentException("Object is not valid IPEndPoint");
			IPEndPoint otherEndPoint = other as IPEndPoint;
			return (otherEndPoint.Address == this.Address && otherEndPoint.Port == this.Port);
		}
		#endregion
	}
}
