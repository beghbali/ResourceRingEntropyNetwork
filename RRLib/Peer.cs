using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace RRLib
{
	/// <summary>
	/// Definitions and routines for a peer
	/// </summary>
	[Serializable]
	public class Peer : ISerializable
	{
		private Node _node;
		public Node node
		{
			get { return _node; }
			set { _node = value; }
		}

		private PublicUserInfo _userInfo;
		public PublicUserInfo userInfo
		{
			get { return _userInfo; }
			set { _userInfo = value; }
		}

		private InformationEntropy[] _IE;
		public InformationEntropy[] IE
		{
			get { return _IE; }
			set { _IE = value; }
		}

		private ulong _token;
		public ulong token
		{
			get { return _token; }
			set { _token = value; }
		}

		public Peer(Node __node, PublicUserInfo __userInfo, InformationEntropy[] __IE)
		{
			node = __node;
			userInfo = __userInfo;
			IE = __IE;
		}
		
		//Deserializer
		public Peer(SerializationInfo info, StreamingContext ctxt)
		{
			node = (Node)info.GetValue("node", typeof(Node));
			userInfo = (PublicUserInfo)info.GetValue("userInfo", typeof(PublicUserInfo));
			IE = (InformationEntropy[])info.GetValue("IE", typeof(InformationEntropy[]));
			token = (ulong)info.GetValue("token", typeof(ulong));
		}
		
		//Serializer
		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("node", node);
			info.AddValue("userInfo", userInfo);
			info.AddValue("IE", IE);
			info.AddValue("token", token);
		}
	}
}
