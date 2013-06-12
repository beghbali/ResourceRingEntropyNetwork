using System;
using System.Net.Sockets;

namespace RRLib
{

	/// <summary>
	/// Summary description for Connection.
	/// </summary>
	public class Connection
	{
		IPEndPoint node;
		TcpClient channel = null;
		UdpClient asyncChannel = null;
		NetworkStream stream = null;

		//For accounting purposes
		private abstract class ConnectionStatistics
		{
			public static uint missedConnections = 0;
			public static uint noReadWritePermissions = 0;
			public static uint numberOfConnections = 0;
		}
		public Connection()
		{
		}

		public Connection(IPEndPoint __node)
		{
			node = __node;
		}

		public UdpClient connect_async()
		{
			try 
			{
				ConnectionStatistics.numberOfConnections++;
				//Establish a connection to the node
				asyncChannel = new UdpClient();
				asyncChannel.Connect(node.GetEndPoint());

				ConnectionStatistics.numberOfConnections++;
			}
			catch
			{
				return null;
			}
			return asyncChannel;
		}

		public NetworkStream connect_sync()
		{
			try 
			{
				ConnectionStatistics.numberOfConnections++;
				//Establish a connection to the node
				channel = new TcpClient();
				channel.Connect(node.GetEndPoint());
				
				//Get a stream to the node
				stream = channel.GetStream();
				ConnectionStatistics.numberOfConnections++;

				if(stream == null)
				{
					ConnectionStatistics.missedConnections++;
					channel.Close();
					return null;
				}
	
				if(!stream.CanRead || !stream.CanWrite)
				{
					ConnectionStatistics.noReadWritePermissions++;
					stream.Close();
					channel.Close();
					return null;
				}
			}
			catch
			{
				return null;
			}
			return stream;
		}

		public void disconnect_async()
		{
			asyncChannel.Close();
		}

		public void disconnect_sync()
		{
			if(stream != null)
				stream.Close();
			channel.Close();
		}
	}
}
