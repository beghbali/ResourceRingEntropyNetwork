using System;
using RRLib;

namespace ResourceRingServer
{
	/// <summary>
	/// Summary description for ServerPolicyManager.
	/// </summary>
	public class ServerPolicyManager
	{
		private Constants.QueryHandlerType queryHandlerPolicy;

		private IPEndPoint _queryServer;
		public IPEndPoint queryServer
		{
			get { return _queryServer; }
			set { _queryServer = value; }
		}

		public ServerPolicyManager()
		{
			//REVISIT: need to read all these data from disk
			this.queryHandlerPolicy = Constants.QueryHandlerType.SERVER_FORM_QUERY;
			if(this.queryHandlerPolicy == Constants.QueryHandlerType.SERVER_FORM_QUERY || 
				this.queryHandlerPolicy == Constants.QueryHandlerType.SERVER_RESOLVE_QUERY)
				queryServer = Constants.SERVER;  //hack
		}

		public Constants.QueryHandlerType getSearchQueryPolicy()
		{
			return this.queryHandlerPolicy;
		}
	}
}
