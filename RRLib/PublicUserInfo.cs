using System;
using System.Runtime.Serialization;

namespace RRLib
{
	/// <summary>
	/// Public User Information
	/// This information may be displayed to other users
	/// </summary>
	[Serializable]
	public class PublicUserInfo : UserInfo, ISerializable
	{
		private string _nickname;
		public string nickname 
		{
			get { return _nickname; }
			set { _nickname = value; }
		}

		public PublicUserInfo()
		{
		}

		public PublicUserInfo(string __nickname)
		{
			nickname = __nickname;
		}

		//Deserializer
		public PublicUserInfo(SerializationInfo info, StreamingContext ctxt)
		{
			nickname = (string)info.GetString("nickname");
		}

		//Serializer
		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("nickname", nickname);
		}
	}
}
