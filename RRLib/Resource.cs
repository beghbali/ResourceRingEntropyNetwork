using System;
using System.IO;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace RRLib
{
	/// <summary>
	/// Resource Header: specifies information about the resource
	/// </summary>
	[Serializable]
	public class ResourceHeader : ISerializable
	{
		#region public variables

		private ulong _resourceID;
		public ulong resourceID
		{
			get { return _resourceID; }
			set { _resourceID = value; }
		}

		private Constants.ResourceType _type;
		public Constants.ResourceType type
		{
			get { return _type; }
			set { _type = value; }
		}

		private uint _tokens;
		public uint tokens
		{
			get { return _tokens; }
			set { _tokens = value; }
		}

		private long _size;
		public long size
		{
			get { return _size; }
			set { _size = value; }
		}

		private string _description;
		public string description
		{
			get { return _description; }
			set { _description = value; }
		}

		private string _name;
		public string name
		{
			get { return _name; }
			set { _name = value; }
		}

		private string _location;
		public string location
		{
			get { return _location; }
			set { _location = value; }
		}
		#endregion

		#region constructors
		public ResourceHeader(ulong __resourceID, Constants.ResourceType __type, uint __tokens, long __size, 
			string __description, string __name, string __location)
		{
			resourceID = __resourceID;
			type = __type;
			tokens = __tokens;
			size = __size;
			description = __description;
			name = __name;
			location = __location;
		}
		#endregion

		#region serialization methods
		//Deserializer
		public ResourceHeader(SerializationInfo info, StreamingContext ctxt)
		{
			resourceID = (ulong)info.GetValue("resourceID", typeof(ulong));
			type = (Constants.ResourceType)info.GetValue("type", typeof(Constants.ResourceType));
			tokens = (uint)info.GetUInt32("tokens");
			size = (long)info.GetInt64("size");
			description = (string)info.GetString("description");
			name = (string)info.GetString("name");
		}
		
		//Serializer
		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("resourceID", resourceID);
			info.AddValue("type", type);
			info.AddValue("tokens", tokens);
			info.AddValue("size", size);
			info.AddValue("description", description);
			info.AddValue("name", name);
		}
		#endregion
	}
	/// <summary>
	/// The base Resource object
	/// </summary>
	public class Resource
	{
		#region private variables
		private static ulong nextResourceID = 0;
		//REVISIT: this needs to be set by the ring lord. A partition of the global resource ids.
		private static ulong maxResourceID = Constants.MAX_RESOURCE_ID;
		#endregion

		#region public variables
		private ResourceHeader _header;
		public ResourceHeader header
		{
			get { return _header; }
			set { _header = value; }
		}

		private InformationEntropy[] _IE;
		public InformationEntropy[] IE
		{
			get { return _IE; }
			set { _IE = value; }
		}

		private object _data;
		public object data
		{
			get { return _data; }
			set { _data = value; }
		}

		byte[] signature;
		TimeSpan timestamp;
		#endregion

		#region constructors
		public Resource()
		{
			header = new ResourceHeader(0,Constants.ResourceType.UNKNOWN_RESOURCE_TYPE,0,0,"", "unknown", "");
		}

		public Resource(ResourceHeader __header)
		{
			header = __header;
		}

		public Resource(Constants.ResourceType __type, uint __tokens, long __size, 
			string __description, string __name, string __location, InformationEntropy[] __IE, object __data)
		{
			header = new ResourceHeader(getNextResourceID(), __type, __tokens, __size, __description, __name, __location);
			IE = __IE;
			data = __data;
		}

		public Resource(ulong __resourceID, Constants.ResourceType __type, uint __tokens, long __size, 
						string __description, string __name, string __location, InformationEntropy[] __IE, object __data)
		{
			header = new ResourceHeader(__resourceID, __type, __tokens, __size, __description, __name, __location);
			IE = __IE;
			data = __data;
		}
		#endregion

		#region static methods

		public static Resource[] filesToResources(FileInfo[] files)
		{
			InformationEntropy[] IE;
			Resource[] resources = new Resource[files.Length];
			int index = 0;
			Constants.ResourceType type = Constants.ResourceType.UNKNOWN_RESOURCE_TYPE;

			foreach (FileInfo file in files)
			{
				if (FileLib.isASCIIDocument(file))
				{
					type = Constants.ResourceType.FILE_TYPE_DOC;
					IE = InvertedIndex.extractInformationEntropy(file);
				}
				else
				{
					type = Resource.determineType(file);
					//just use the file name as an indicator of info in the file
					IE = InvertedIndex.extractInformationEntropy(file.Name);
				}

				Debug.Assert((IE != null), "Information Entropy from " + file.Name + " is null");
				resources[index++] = new Resource(type, Constants.DEFAULT_TOKENS, file.Length, 
												  InformationEntropy.Detokenize(IE, ' '), file.Name, file.FullName, IE, file);
			}
			return resources;
		}

		public static Constants.ResourceType determineType(FileInfo file)
		{
			int index = 0;
			foreach (string[] resourceGroup in Constants.RESOURCE_FILTERS)
			{
				foreach (string extension in resourceGroup)
				{
					if (extension == file.Extension)
						return ((Constants.ResourceType)index);
				}
			}
			return Constants.ResourceType.UNKNOWN_RESOURCE_TYPE;
		}

		public static ulong getNextResourceID()
		{
			if (nextResourceID >= maxResourceID)
				return 0;
			return ++nextResourceID;
		}

		public static void setMaxResourceID(ulong max)
		{
			maxResourceID = max;
		}
		#endregion
	}
}
