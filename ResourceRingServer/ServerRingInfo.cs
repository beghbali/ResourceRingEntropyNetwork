using System;
using RRLib;

namespace ResourceRingServer
{
	public class ServerRingInfo
	{
		private ulong nextSessionID;

		private Ring _ring;
		public Ring ring
		{
			get { return _ring; }
			set { _ring = value; }
		}

		private ClientManager _ringClientManager;
		public ClientManager ringClientManager
		{
			get { return _ringClientManager; }
			set { _ringClientManager = value; }
		}

		private AccountsManager _ringAccountsManager;
		public AccountsManager ringAccountsManager
		{
			get { return _ringAccountsManager; }
			set { _ringAccountsManager = value; }
		}

		private PeerManager _ringPeerManager;
		public PeerManager ringPeerManager
		{
			get { return _ringPeerManager; }
			set { _ringPeerManager = value; }
		}

		private ServerPolicyManager _serverPolicyManager;
		public ServerPolicyManager serverPolicyManager
		{
			get { return _serverPolicyManager; }
			set { _serverPolicyManager = value; }
		}

		public ServerRingInfo(Ring __ring, ClientManager __ringClientManager, 
			AccountsManager __ringAccountsManager, PeerManager __ringPeerManager, 
			ServerPolicyManager __serverPolicyManager)
		{
			ring = __ring;
			ringClientManager = __ringClientManager;
			ringAccountsManager = __ringAccountsManager;
			ringPeerManager = __ringPeerManager;
			serverPolicyManager = __serverPolicyManager;
			nextSessionID = 1;
		}

		public ulong getNextSessionID()
		{
			return nextSessionID++;
		}

		public static ServerRingInfo findServerRingInfoByID(ServerRingInfo[] serverRingsInfo, uint ringID)
		{
			for(uint serverRingInfo = 0; serverRingInfo < serverRingsInfo.Length; serverRingInfo++)
			{
				if(serverRingsInfo[serverRingInfo].ring.ringID == ringID)
					return serverRingsInfo[serverRingInfo];
			}
			return null;
		}
	}
}
