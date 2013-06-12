using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using RRLib;

namespace ResourceRingServer
{
	/// <summary>
	/// Manages user accounts
	/// </summary>
	public class AccountsManager
	{
		#region private member variables
		private Hashtable accounts;
		private Hashtable userIDNameTable;
		private string accountsFile;
		#endregion

		#region constructors
		public AccountsManager(string __accountsFile)
		{
			accounts = new Hashtable();
			userIDNameTable = new Hashtable();
			accountsFile = __accountsFile;
		}
		#endregion
		
		static uint userIDCounter = 1000;
		public void loadAccounts()
		{
			string[] memberNames = {"bosnia", "albania", "france", "germany", "italy", "poland", 
									   "romania", "spain", "greece", "turkey", "israel", "palestine",
									   "jordan", "syria", "iraq", "iran", "afghanistan", "pakistan", 
									   "india", "russia", "china", "indonesia", "taiwan", "vietnam", 
									   "thailand", "japan", "austrailia", "brazil", "peru", "mexico"};
			
			Random randomizer = new Random(System.DateTime.Now.Millisecond);
			Member[] members = new Member[memberNames.Length];
			int index = 0;
			foreach (string name in memberNames)
			{
				Member member = new Member(name, ASCIIEncoding.ASCII.GetBytes(name), 
					new PublicUserInfo(name), new PrivateUserInfo(userIDCounter++, "Mr.", "jose",
				"garcia", "morena", "III", "23 El dorado Rd", "San Pablo", 123477, "California", name), 
					new Node(new IPEndPoint(System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList[0],
					1024 + randomizer.Next() % 14000), new IPEndPoint(System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList[0],
					1024 + randomizer.Next() % 14000), Constants.LineSpeedType.LNSPEED_DSL));
				member.lords = Constants.DEFAULT_LORDS;
				members[index++] = member;
			}
			
			TextWriter writer = new StreamWriter(accountsFile);
			XmlSerializer serializer = new XmlSerializer(typeof(Member[]));
			serializer.Serialize(writer, members);
			writer.Close();

			XmlReader reader = new XmlTextReader(accountsFile);
			XmlSerializer deserializer = new XmlSerializer(typeof(Member[]));
			Member[] existingMembers = (Member[])deserializer.Deserialize(reader);
			foreach (Member existingMember in existingMembers)
			{
				addAccount(existingMember.userName, existingMember.password, existingMember);
			}
			reader.Close();
		}

		public bool addAccount(string userName, byte[] password, Member user)
		{
			Member newMember = new Member(userName, password, user.publicUserInfo, user.privateUserInfo, user.node,user.lords);
			if(accounts.Contains(user.privateUserInfo.userID))
				return false;
			accounts.Add(user.privateUserInfo.userID, newMember);
			userIDNameTable.Add(userName, user.privateUserInfo.userID);
			return true;
		}

		public bool updateAccount(string userName, byte[] password, Member user)
		{
			Member newMember = new Member(userName, password, user.publicUserInfo, user.privateUserInfo, user.node, user.lords);
			if(accounts.Contains(user.privateUserInfo.userID))
				return false;

			Member member;
			if((member = findMemberByUserName(userName)) != null)
			{
				userIDNameTable.Remove(userName);
				userIDNameTable.Add(userName, user.privateUserInfo.userID);
			}
			accounts[member.privateUserInfo.userID] = newMember;
			return true;
		}

		private Member getMemberInternal(uint userID)
		{
			Member registeredUserAccount = (Member)accounts[userID];
			if(registeredUserAccount == null)
				return null;

			return registeredUserAccount;
		}

		public bool isMember(uint userID)
		{
			return (getMemberInternal(userID) != null);
		}

		public Member findMemberByUserID(uint userID)
		{
			return getMemberInternal(userID);
		}

		public bool isMember(string userName)
		{
			return (findMemberByUserName(userName) != null);
		}

		public Member findMemberByUserName(string userName)
		{
			object userIDobj = userIDNameTable[userName];
			if(userIDobj == null)
				return null;
			
			return getMemberInternal((uint)userIDobj);
		}
	}
}
