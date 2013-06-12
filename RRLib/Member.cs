using System;

namespace RRLib
{
	/// <summary>
	/// Summary description for Member.
	/// </summary>
	public class Member
	{
		#region public member variables
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

		private LordInfo[] _lords;
		public LordInfo[] lords 
		{
			get { return _lords; }
			set { _lords = value; }
		}
		#endregion

		#region constructors 
		public Member()
		{
		}

		public Member(string __userName, byte[] __password, PublicUserInfo pubInfo, PrivateUserInfo privInfo, Node __node, LordInfo[] __lords)
		{
			userName = __userName;
			password = __password;
			publicUserInfo = pubInfo;
			privateUserInfo = privInfo;
			node = __node;
			lords = __lords;
		}

		public Member(string __userName, byte[] __password, PublicUserInfo pubInfo, PrivateUserInfo privInfo, Node __node)
		{
			userName = __userName;
			password = __password;
			publicUserInfo = pubInfo;
			privateUserInfo = privInfo;
			node = __node;
		}
		#endregion
	}
}
