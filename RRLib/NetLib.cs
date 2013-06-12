using System;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using System.Text;
using System.Xml;

namespace RRLib
{
	public delegate void handleConnection(BinaryReader reader, BinaryWriter writer);
	/// <summary>
	/// Library for network operations
	/// </summary>
	public abstract class NetLib
	{
		public class ConnectionState
		{
			private handleConnection _handler;
			public handleConnection handler
			{
				get { return _handler; }
				set { _handler = value; }
			}

			private Socket _socket;
			public Socket socket
			{
				get { return _socket; }
				set { _socket = value; }
			}

			private byte[] _buffer;
			public byte[] buffer
			{
				get { return _buffer; }
				set { _buffer = value; }
			}

			private int _position;
			public int position
			{
				get { return _position; }
				set { _position = value; }
			}

			public ConnectionState(handleConnection __handler, Socket __socket, byte[] __buffer)
			{
				handler = __handler;
				socket = __socket;
				buffer = __buffer;	   
				position = 0;
			}
		}

		public static ManualResetEvent UdpConnectionDone = new ManualResetEvent(false);
		public static ManualResetEvent TcpConnectionDone = new ManualResetEvent(false);

		private static uint nextMessageID;

		public static System.Net.IPAddress getHostIPAddress()
		{
			return System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList[0];
		}


		public static NetworkStream OpenCommunicationChannel(IPEndPoint node)
		{
			Connection nodeConnection = null;
			NetworkStream nodeStream = null;

			try
			{
				//connect to node
				nodeConnection = new Connection(node);
				nodeStream = nodeConnection.connect_sync();
				if(nodeStream == null)
				{
					nodeConnection.disconnect_sync();
					return null;
				}
			}
			catch (Exception e)
			{
				int x = 2;

				if(nodeConnection != null)
					nodeConnection.disconnect_sync();
			}

			return nodeStream;
		}

		/// <summary>
		/// Connects to the end point (node) and sends message to it. If waitForReply is set
		/// it will wait for a reply from the node and return the reply
		/// </summary>
		/// <param name="node">The end point (node) in the network to communicate with</param>
		/// <param name="message">Message to send to node</param>
		/// <param name="waitForReply">Whether a reply is expected</param>
		/// <returns></returns>
		public static byte[] communicate(IPEndPoint node, byte[] message, bool waitForReply)
		{
			NetworkStream nodeStream = null;
			BinaryReader reader = null;
			BinaryWriter writer = null;
			byte[] reply = new byte[Constants.READBUFFSIZE];
			int bytesRead = 0;

			try 
			{
				nodeStream = OpenCommunicationChannel(node);
				
				reader = new BinaryReader(nodeStream);
				writer = new BinaryWriter(nodeStream);

				writer.Write(message);
				writer.Flush();
				
				if(waitForReply)
					bytesRead = reader.Read(reply, 0, reply.Length);
						
				reader.Close();
				writer.Close();
			}
			catch (Exception e)
			{
				return null;
			}
			finally
			{
				nodeStream.Close();
			}

			if(bytesRead == 0)
				return null;

			return reply;
		}


		/// <summary>
		/// Connects to the end point (node) and sends message to it. If waitForReply is set
		/// it will wait for a reply from the node and return the reply. It uses Asynchronous UDP
		/// protocol
		/// </summary>
		/// <param name="node">The end point (node) in the network to communicate with</param>
		/// <param name="message">Message to send to node</param>
		/// <param name="waitForReply">Whether a reply is expected</param>
		/// <returns></returns>
		public static byte[] communicateAsync(IPEndPoint node, byte[] message, bool waitForReply)
		{
			Connection nodeConnection = null;
			UdpClient udpClient = null;
			byte[] reply = null;

			try 
			{
				//connect to node
				nodeConnection = new Connection(node);
				udpClient = nodeConnection.connect_async();
				if(udpClient == null)
				{
					nodeConnection.disconnect_async();
					return null;
				}

				udpClient.Send(message, message.Length);
				System.Net.IPEndPoint senderEndPoint = new System.Net.IPEndPoint(0,0);

				if(waitForReply)
					reply = udpClient.Receive(ref senderEndPoint);
			}
			catch (Exception e)
			{
				return null;
			}
			finally
			{
				nodeConnection.disconnect_async();
			}

			if(reply != null && reply.Length == 0)
				return null;

			return reply;
		}

		public static void listenAndCommunicate(IPEndPoint localEndPoint, handleConnection handler)
		{
			TcpListener TCPListener = null;
			Socket newConnection = null;
			NetworkStream clientStream = null;
			BinaryReader reader = null;
			BinaryWriter writer = null;
			byte[] buffer = new byte[Constants.READBUFFSIZE];

			try
			{
				TCPListener = new TcpListener(localEndPoint.GetEndPoint());
				TCPListener.Start();
				
				while(true)
				{
					newConnection = TCPListener.AcceptSocket();
					//newConnection.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.KeepAlive, 
					//	1);
					
					if(!newConnection.Connected)
					{
						newConnection.Close();
						continue;
					}
		
					//Get streams to the client so we can read and write to it.
					clientStream = new NetworkStream(newConnection);
					//If we can't read and write to the client abort connection
					if(!clientStream.CanRead || !clientStream.CanWrite)
					{
						clientStream.Close();
						newConnection.Close();
						continue;
					}
					reader = new BinaryReader(clientStream);
					writer = new BinaryWriter(clientStream);

					//newConnection.Receive(buffer, 0, buffer.Length, SocketFlags.None);
					 reader.Read(buffer, 0, buffer.Length);
					
					reader = new BinaryReader(new MemoryStream(buffer));

					//process the message
					handler(reader, writer);	
					
					reader.Close();
					writer.Close();
					clientStream.Close();
					newConnection.Close();
				}
			}
			catch (Exception e)
			{
				int x = 2;
			}
			finally
			{
				if(clientStream != null)
					clientStream.Close();
				if(newConnection != null)
					newConnection.Close();
			}
		}

		/*public static void listenAndCommunicate(IPEndPoint localEndPoint, handleConnection handler)
		{
			TcpClient client;
			byte[] buffer = new byte[Constants.READBUFFSIZE];
			MemoryStream stream = new MemoryStream(buffer);

			try
			{
				TcpListener listener = new TcpListener(localEndPoint);
				listener.Start();

				//ConnectionState state = new ConnectionState(handler, socket, buffer);
				while (true)
				{
					TcpConnectionDone.Reset();
					client = listener.AcceptTcpClient();
					if (client.GetStream().CanRead)
					{
						client.GetStream().Read(buffer, 0, buffer.Length);
						
						handler(new BinaryReader(stream), new BinaryWriter(stream));
						//client.GetStream().BeginRead(state.buffer, 0, state.buffer.Length, SocketFlags.None, 
						//	new AsyncCallback(ReceiveCallback), state);

						//socket.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, 
						//	new AsyncCallback(ReceiveCallback), state);
						TcpConnectionDone.WaitOne();
					}
				}
			}
			catch (Exception e)
			{
				int x = 2;
			}
		}*/

		public static void listenAndCommunicateAsync(IPEndPoint localEndPoint, handleConnection handler)
		{
			Socket socket;
			byte[] buffer = new byte[Constants.READBUFFSIZE];

			try
			{
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				socket.Bind(localEndPoint.GetEndPoint());
				ConnectionState state = new ConnectionState(handler, socket, buffer);
				while (true)
				{
					UdpConnectionDone.Reset();
					socket.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, 
						new AsyncCallback(ReceiveCallback), state);
					UdpConnectionDone.WaitOne();
				}
			}
			catch (Exception e)
			{
				int x = 2;
			}
		}
		
		private static void AcceptCallback (IAsyncResult asyn)
		{
			TcpConnectionDone.Set();

			ConnectionState state = (ConnectionState)asyn.AsyncState;
			Socket socket = state.socket.EndAccept(asyn);
			state.socket = socket;

			socket.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback),
				state);
		}

		private static void ReceiveCallback (IAsyncResult asyn)
		{
			BinaryReader reader = null;
			BinaryWriter writer = null;
			MemoryStream stream;
			int bytesRead;

			UdpConnectionDone.Set();
			TcpConnectionDone.Set();
			ConnectionState state = (ConnectionState)asyn.AsyncState;

			bytesRead = state.socket.EndReceive(asyn);

			if (bytesRead > 0)
			{
				stream = new MemoryStream(state.buffer);
				reader = new BinaryReader(stream);
				//can't write, this is connection-less and not stream oriented

				state.position += bytesRead;

				state.handler(reader, writer);
				//get the rest of the message if anything left
				//AsyncCallback udpHandler = new AsyncCallback (ReceiveCallback);
				//state.socket.BeginReceive(state.buffer, state.position, state.buffer.Length, SocketFlags.None, udpHandler, 
				//	state);
			}
			//else if (state.position > 0)
				//state.handler(reader, writer);
		}

		public static void insertEntropyHeader(BinaryFormatter serializer, MemoryStream stream)
		{
			if(nextMessageID == 0)
			{
				Random r = new Random(DateTime.Now.Millisecond);
				nextMessageID = (uint)r.Next();
			}
			serializer.Serialize(stream, Constants.ENTROPY_PROTOCOL_VERSION);
			serializer.Serialize(stream, ++nextMessageID);
		}

		public static void bypassEntropyHeader(BinaryFormatter deserializer, Stream stream)
		{
			string version = (string)deserializer.Deserialize(stream);
			uint messageID = (uint)deserializer.Deserialize(stream);
		}
	}

}
