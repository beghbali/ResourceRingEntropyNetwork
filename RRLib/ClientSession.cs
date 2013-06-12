using System;

namespace RRLib
{
	/// <summary>
	/// Summary description for ClientSession.
	/// </summary>
	public class ClientSession
	{
		private Member _user;
		public Member user
		{ 
			get { return _user; }
			set { _user = value; }
		}
			
		private ulong _sessionID;
		public ulong sessionID
		{ 
			get { return _sessionID; }
			set { _sessionID = value; }
		}

		public ClientSession(Member __user, ulong __sessionID)
		{
			user = __user;
			sessionID = __sessionID;
		}
	}
}
