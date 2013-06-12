using System;
using System.Runtime.Serialization;

namespace RRLib
{
	/// <summary>
	/// Description of a node in the network
	/// </summary>
	[Serializable]
	public class Node : ISerializable
	{
		#region public member variables
		private IPEndPoint _syncCommunicationPoint;
		public IPEndPoint syncCommunicationPoint
		{
			get { return _syncCommunicationPoint; }
			set { _syncCommunicationPoint = value; }
		}

		private IPEndPoint _asyncCommunicationPoint;
		public IPEndPoint asyncCommunicationPoint
		{
			get { return _asyncCommunicationPoint; }
			set { _asyncCommunicationPoint = value; }
		}

		private Constants.LineSpeedType _lineSpeed;
		public Constants.LineSpeedType lineSpeed
		{
			get { return _lineSpeed; }
			set { _lineSpeed = value; }
		}
		#endregion

		#region constrcutors
		public Node()
		{
		}

		public Node(IPEndPoint syncCommPoint, IPEndPoint asyncCommPoint, Constants.LineSpeedType __lineSpeed)
		{
			syncCommunicationPoint = syncCommPoint;
			asyncCommunicationPoint = asyncCommPoint;
			lineSpeed = __lineSpeed;
		}
		#endregion

		#region serialization methods
		//Deserializer
		public Node(SerializationInfo info, StreamingContext ctxt)
		{
			syncCommunicationPoint = (IPEndPoint)info.GetValue("syncCommPoint", typeof(IPEndPoint));
			asyncCommunicationPoint = (IPEndPoint)info.GetValue("asyncCommPoint", typeof(IPEndPoint));
			lineSpeed = (Constants.LineSpeedType)info.GetValue("lineSpeed", typeof(Constants.LineSpeedType));
		}

		//Serializer
		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("syncCommPoint", syncCommunicationPoint);
			info.AddValue("asyncCommPoint", asyncCommunicationPoint);
			info.AddValue("lineSpeed", lineSpeed);
		}
		#endregion
	}
}
