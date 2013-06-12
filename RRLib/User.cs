using System;
using System.Runtime.Serialization;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace RRLib
{
	/// <summary>
	/// Summary description for User.
	/// </summary>
	[Serializable]
	public class User : ISerializable
	{
		#region private static variables
		private static User userinstance;
		#endregion

		#region public member variables
		private bool _loggedIn;
		public bool loggedIn
		{
			get { return _loggedIn; }
			set { _loggedIn = value; }
		}

		private RingInfo[] _ringsInfo;
		public RingInfo[] ringsInfo
		{
			get { return _ringsInfo; }
			set { _ringsInfo = value; }
		}
		
		private PublicUserInfo _publicUserInfo;
		public PublicUserInfo publicUserInfo
		{
			get { return _publicUserInfo; }
			set { _publicUserInfo = value; }
		}

		private PrivateUserInfo _privateUserInfo;
		public PrivateUserInfo privateUserInfo
		{
			get { return _privateUserInfo; }
			set { _privateUserInfo = value; }
		}

		private Node _node;
		public Node node
		{
			get { return _node; }
			set { _node = value; }
		}
		#endregion

		#region constructors
		public User(string[] usernames, string[] passwords)
		{
			if(usernames.Length != passwords.Length)
				return;

			ringsInfo = new RingInfo[usernames.Length];
			for(int ringIndx = 0; ringIndx < usernames.Length; ringIndx++)
			{
				ringsInfo[ringIndx] = new RingInfo(null,usernames[ringIndx], passwords[ringIndx]);
			}

			publicUserInfo = new PublicUserInfo();
			privateUserInfo = new PrivateUserInfo();
			loggedIn = false;
		}

		public User(RingInfo[] __ringsInfo, PublicUserInfo __publicUserInfo, PrivateUserInfo __privateUserInfo, Node __node)
		{
			ringsInfo = __ringsInfo;
			publicUserInfo = __publicUserInfo;
			privateUserInfo = __privateUserInfo;
			node = __node;
			loggedIn = false;
		}
		#endregion

		#region serialization methods
		//Deserializer
		public User(SerializationInfo info, StreamingContext ctxt)
		{
			ringsInfo = (RingInfo[])info.GetValue("ringsInfo", typeof(RingInfo[]));
			publicUserInfo = (PublicUserInfo)info.GetValue("publicUserInfo", typeof(PublicUserInfo));
			privateUserInfo = (PrivateUserInfo)info.GetValue("privateUserInfo", typeof(PrivateUserInfo));
			node = (Node)info.GetValue("node", typeof(Node));
		}

		//Serializer
		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("ringsInfo", ringsInfo);
			info.AddValue("publicUserInfo", publicUserInfo);
			info.AddValue("privateUserInfo", privateUserInfo);
			info.AddValue("node", node);
		}
		#endregion

		#region public member methods
		public void changeInfo(PublicUserInfo _publicUserInfo, PrivateUserInfo _privateUserInfo)
		{
			publicUserInfo = _publicUserInfo;
			privateUserInfo = _privateUserInfo;
		}

		public bool isSameAs(User another)
		{
			return true;
		}

		public RingInfo findRingInfo(uint ringID)
		{
			foreach (RingInfo ringInfo in ringsInfo)
			{
				if(ringInfo.ring.ringID == ringID)
					return ringInfo;
			}
			return null;
		}
		#endregion

		#region public static methods
		public static User getInstance()
		{
			//singleton pattern
			if(userinstance == null)
			{
				//REVISIT: needs to get the file ptr from global client configurationManager
				XmlReader reader = new XmlTextReader(@"..\..\..\ResourceRingServer\bin\Debug\accounts.xml");
				XmlSerializer deserializer = new XmlSerializer(typeof(Member[]));
				Member[] existingMembers = (Member[])deserializer.Deserialize(reader);
				reader.Close();

				Random randomizer = new Random(System.DateTime.Now.Millisecond);
				Member member = existingMembers[randomizer.Next()%existingMembers.Length];
				userinstance = User.memberToUser(member);
				userinstance.ringsInfo = Constants.DEFAULT_RINGSINFO;

				foreach (RingInfo ringInfo in userinstance.ringsInfo)
				{
					ringInfo.userName = member.userName;
					ringInfo.password = member.password;
					ringInfo.IE = new InformationEntropy[1];
					ringInfo.IE[0] = new InformationEntropy(ringInfo.ring.ringName, 1.0f);
				}
			}
			return userinstance;
		}

		public static void setInstance(User _user)
		{
			userinstance = _user;
		}

		public static User memberToUser(Member member)
		{
			return (new User(null, member.publicUserInfo, member.privateUserInfo, member.node));
		}
		#endregion
	}
}
