using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net.Sockets;

namespace RRLib
{
	/// <summary>
	/// Summary description for DownloadManager.
	/// </summary>
	public class DownloadManager
	{
		private Download[] downloads;
		private Queue pendingDownloads;

		public DownloadManager()
		{
			downloads = new Download[Constants.MAX_SIMULTANEOUS_DOWNLOADS];
			pendingDownloads = new Queue();
		}

		public bool Download (Peer peer, RingInfo ringInfo, ResourceHeader resourceHeader, 
			UICallBack UIHndlr)
		{
			Download download = new Download(peer, ringInfo, resourceHeader, UIHndlr);
			int downloadIndex = Add(download);
			if (downloadIndex < 0)
			{
				Queue(download);
				return false;
			}
			Download_Internal(download);
			return true;
		}

		public void Download_Internal (Download download)
		{
			//REVISIT:first try downloading from single source and then try parallelizing
			byte[] message = new byte[Constants.WRITEBUFFSIZE];
			byte[] buffer = new Byte[Constants.READBUFFSIZE];
			MemoryStream stream;
			BinaryReader reader = null;
			BinaryWriter writer = null;
			
			//REVISIT: when implementing parallel downloads, need to change these dynamically
			int offset = 0;
			int bytesToDownload = (int)download.header.size;

			try 
			{
				BinaryFormatter serializer = new BinaryFormatter();
				stream = new MemoryStream(message);
				
				NetLib.insertEntropyHeader(serializer, stream);

				serializer.Serialize(stream, Constants.MessageTypes.MSG_DOWNLOADREQUEST);
				serializer.Serialize(stream, download.ringInfo.token);
				serializer.Serialize(stream, download.ringInfo.ring.ringID);
				serializer.Serialize(stream, User.getInstance().node.syncCommunicationPoint);
				serializer.Serialize(stream, download.header);
				serializer.Serialize(stream, offset);
				serializer.Serialize(stream, bytesToDownload);

				//Send out the request for download
				NetworkStream networkStream = NetLib.OpenCommunicationChannel(download.peer.node.syncCommunicationPoint);

				reader = new BinaryReader(networkStream);
				writer = new BinaryWriter(networkStream);

				writer.Write(message);
				writer.Flush();

				stream.Close();

				reader.Read(buffer, 0, buffer.Length);
				BinaryFormatter deserializer = new BinaryFormatter();

				stream = new MemoryStream(buffer);
				NetLib.bypassEntropyHeader(deserializer, stream);
				Constants.MessageTypes replyMsg = (Constants.MessageTypes)deserializer.Deserialize(stream);
						
				switch(replyMsg)
				{
					case Constants.MessageTypes.MSG_DOWNLOADREPLY:
						long leftToDownload = download.header.size;
						byte[] firstResourceChunk = (byte[])deserializer.Deserialize(stream);
						
						//Insert the initial chunk into the download buffer.
						//REVISIT: figure a way to just deserialize into download.buffer (avoids a copy)
						firstResourceChunk.CopyTo(download.buffer, 0);

						long bytesReadOnThisRead = firstResourceChunk.Length;
						download.bytesRead += bytesReadOnThisRead;
						leftToDownload -= bytesReadOnThisRead;
						download.UIHandler.Invoke(bytesReadOnThisRead);

						while (leftToDownload > 0 && bytesReadOnThisRead > 0)
						{
							bytesReadOnThisRead = reader.Read(download.buffer, (int)download.bytesRead, 
								download.buffer.Length-(int)download.bytesRead);
							download.bytesRead += bytesReadOnThisRead;
							leftToDownload -= bytesReadOnThisRead;
							download.UIHandler.Invoke(bytesReadOnThisRead);
							System.Threading.Thread.Sleep(10);
						}

						Debug.Assert(download.bytesRead == download.header.size, "Did not receive the entire file");
						break;
					default:
						Remove(download);
						Queue(download);
						break;
				}
			}
			catch (Exception e)
			{
				int x = 2;  //REVISIT: probably need to send info to logger.
			}
			finally
			{
				if (reader != null && writer != null)
				{
					reader.Close();
					writer.Close();
				}
			}
				
		}

		public void HandleDownloadRequest (BinaryReader reader, BinaryWriter writer)
		{
			try 
			{
				BinaryFormatter deserializer = new BinaryFormatter();
				ulong token = (ulong)deserializer.Deserialize(reader.BaseStream);
				uint ringID = (uint)deserializer.Deserialize(reader.BaseStream);
				IPEndPoint requester = (IPEndPoint)deserializer.Deserialize(reader.BaseStream);
				ResourceHeader header = (ResourceHeader)deserializer.Deserialize(reader.BaseStream);
				int offset = (int)deserializer.Deserialize(reader.BaseStream);
				int count = (int)deserializer.Deserialize(reader.BaseStream);

				RingInfo ringInfo = RingInfo.findRingInfoByID(User.getInstance().ringsInfo, ringID);
				ResourceGroup group = ringInfo.cataloger.GetResourceGroup(new Resource(header));
				if (group == null)
				{
					InformPeerThatResourceWasNotFound(reader, writer);
					return;
				}

				Resource resourceToBeDownloaded = (Resource)group.resourceList[header.resourceID];
			
				Stream fileStream = File.OpenRead(resourceToBeDownloaded.header.location); 

				byte[] message = new byte[Constants.WRITEBUFFSIZE];
				byte[] buffer = new Byte[count];
				long bytesSent = 0;
				long bytesRead = 0;

				//MemoryStream stream = new MemoryStream(message);
				MemoryStream stream = new MemoryStream(message);
				
				stream.Seek(offset, SeekOrigin.Begin);

				BinaryFormatter serializer = new BinaryFormatter();
				
				NetLib.insertEntropyHeader(serializer, stream);
				serializer.Serialize(stream, Constants.MessageTypes.MSG_DOWNLOADREPLY);
				
				Debug.Assert(buffer.Length >= count, "File read buffer not sufficient");

				bytesRead = fileStream.Read(buffer, (int)bytesSent, count);
				
				Debug.Assert(bytesRead == count, "Did not read entire file");
			
				while (bytesSent < count)
				{
					//28 is a cushion left for byte[] serialization parameters like size to get serialized on the stream
					long bytesThatFit = stream.Length - stream.Position - 28;
					byte[] serializeBuffer = LibUtil.SliceArray(buffer, bytesSent, bytesThatFit);
						
					//REVISIT: This is horrible. We need to read directly into buffer and be able to specify a portion of
					//it to be serialized. We should get rid of serializeBuffer
					serializer.Serialize(stream, serializeBuffer);
			
					//No Entropy header? nothing????
					writer.Write(message);
						
					//Length of the buffer and not bytesThatFit is the actual number of bytes written
					bytesSent += serializeBuffer.Length;
					stream.Position = 0;
				}
				writer.Flush();
				fileStream.Close();

				Debug.Assert(bytesSent == count, "Did not send the entire file");
				
			}
			catch (Exception e)
			{
				int x = 2;
			}
		}

		private void InformPeerThatResourceWasNotFound (BinaryReader reader, BinaryWriter writer)
		{
			byte[] message = new byte[Constants.WRITEBUFFSIZE];
			MemoryStream stream;

			BinaryFormatter serializer = new BinaryFormatter();
			stream = new MemoryStream(message);
				
			NetLib.insertEntropyHeader(serializer, stream);

			serializer.Serialize(stream, Constants.MessageTypes.MSG_RESOURCENOTFOUND);

			writer.Write(message);
			writer.Flush();
			stream.Close();
		}

		private int Add (Download download)
		{
			for (int index = 0; index < downloads.Length; index++)
			{
				if (downloads[index] == null)
				{
					downloads[index] = download;
					return index;
				}
			}
			return -1;
		}

		private void Remove (Download download)
		{
			for (int index = 0; index < downloads.Length; index++)
			{
				if (downloads[index] == download)
					downloads[index] = null;
			}
		}

		private void Queue (Download download)
		{
			pendingDownloads.Enqueue(download);
		}
	}
}
