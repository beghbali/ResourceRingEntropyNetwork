using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using System.Windows.Forms;
using RRLib;

namespace ResourceRing
{
	public delegate void setSessionIDHandler(ulong _sessionID);
	public delegate ulong getSessionIDHandler();
	public delegate void addAdjacentPeersHandler(Peer[] adjPeers);

	/// <summary>
	/// Summary description for Login.
	/// </summary>
	public class Login
	{
		public struct loginCallbackTable_
		{
			public setSessionIDHandler setSessionID;
			public getSessionIDHandler getSessionID;
			public addAdjacentPeersHandler addAdjacentPeers;
		};

		public loginCallbackTable_ loginCallbackTable;
		
		private bool _loggedIn;
		public bool loggedIn
		{
			get { return _loggedIn; }
			set { _loggedIn = value; }
		}

		private abstract class LoginStatistics
		{
			public static uint numberOfTimesConnectedToServer = 0;
			public static uint numberOfErrorAcks = 0;
		}

		public Login()
		{
			loggedIn = false;
		}

		public Constants.LoginStatus logon(User user)
		{
			Constants.LoginStatus retval = Constants.LoginStatus.STATUS_SERVERNOTREACHED;
			byte[] message = new byte[Constants.WRITEBUFFSIZE];
			byte[] reply;
			MemoryStream stream = null;

			try
			{
				//Serialize data in memory so you can send them as a continuous stream
				BinaryFormatter serializer = new BinaryFormatter();
				stream = new MemoryStream(message);
				NetLib.insertEntropyHeader(serializer, stream);

				serializer.Serialize(stream, Constants.MessageTypes.MSG_LOGIN);
				serializer.Serialize(stream, user.ringsInfo[0].userName);
				serializer.Serialize(stream, user.ringsInfo[0].password);
				serializer.Serialize(stream, user.node);
				reply = NetLib.communicate(Constants.SERVER,message, true);
				stream.Close();
				stream = new MemoryStream(reply);
				Constants.MessageTypes replyMsg = (Constants.MessageTypes)serializer.Deserialize(stream);

				switch(replyMsg)
				{
					case Constants.MessageTypes.MSG_OK:
						retval = Constants.LoginStatus.STATUS_LOGGEDIN;
						ulong sessionID = (ulong)serializer.Deserialize(stream);
						int numAdjPeers = (int)serializer.Deserialize(stream);
						Peer[] adjPeers = null;
						if(numAdjPeers > 0)
						{
							adjPeers = new Peer[numAdjPeers];
							adjPeers = (Peer[])serializer.Deserialize(stream);
						}

						//add the adjacent peers reported to our list (callback to Client)
						this.loginCallbackTable.addAdjacentPeers(adjPeers);
						loggedIn = true;

						//set the session ID (callback to ServerProxy)
						this.loginCallbackTable.setSessionID(sessionID);
						break;
					case Constants.MessageTypes.MSG_NOTAMEMBER:
						retval = Constants.LoginStatus.STATUS_NOTAMEMBER;
						break;
					case Constants.MessageTypes.MSG_ALREADYSIGNEDIN:
						retval = Constants.LoginStatus.STATUS_ALREADYSIGNEDIN;
						break;
					default:
						break;
				}
			}
			catch (Exception e)
			{
				
				int x = 2;
			}

			return retval;
		}

		public void logoff(User user)
		{
			byte[] message = new byte[Constants.WRITEBUFFSIZE];
			MemoryStream stream = null;

			if(!loggedIn)
				return;
			try 
			{
				//Serialize data in memory so you can send them as a continuous stream
				BinaryFormatter serializer = new BinaryFormatter();
				stream = new MemoryStream(message);
				serializer.Serialize(stream, Constants.MessageTypes.MSG_LOGOFF);
				serializer.Serialize(stream, this.loginCallbackTable.getSessionID());
				serializer.Serialize(stream, user.publicUserInfo);
				serializer.Serialize(stream, user.privateUserInfo);
				serializer.Serialize(stream, user.node);
				NetLib.communicate(Constants.SERVER,message, false);
				stream.Close();
			}
			catch
			{
				return;
			}
			finally
			{
				loggedIn = false;
			}
		}

		//is username and password saved on this machine
		public static bool isUserNameAndPasswordSaved()
		{
			return true;
		}

	}
}
