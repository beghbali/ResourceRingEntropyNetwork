using System;
using System.Runtime.Serialization;

namespace RRLib
{
	/// <summary>
	/// Private User Information
	/// This information may _NOT_ be displayed to other users
	/// </summary>
	[Serializable]
	public class PrivateUserInfo : UserInfo, ISerializable
	{
		protected uint _userID;
		public uint userID
		{
			get { return _userID; }
			set { _userID = value; }

		}

		private string _prefix;
		public string prefix 
		{
			get { return _prefix; }
			set { _prefix = value; }

		}
		
		private string _firstName;
		public string firstName
		{
			get { return _firstName; }
			set { _firstName = value; }

		}

		private string _middleName;
		public string middleName
		{
			get { return _middleName; }
			set { _middleName = value; }

		}

		private string _lastName;
		public string lastName
		{
			get { return _lastName; }
			set { _lastName = value; }

		}

		private string _suffix;
		public string suffix
		{
			get { return _suffix; }
			set { _suffix = value; }
		}

		private string _streetAddress;
		public string streetAddress
		{
			get { return _streetAddress; }
			set { _streetAddress = value; }
		}

		private string _city;
		public string city
		{
			get { return _city; }
			set { _city = value; }
		}

		private uint _zipCode;
		public uint zipCode
		{
			get { return _zipCode; }
			set { _zipCode = value; }
		}
	
		private string _state;
		public string state
		{
			get { return _state; }
			set { _state = value; }
		}

		private string _country;
		public string country
		{
			get { return _country; }
			set { _country = value; }
		}

		public PrivateUserInfo()
		{
			userID = Constants.INVALIDUSERID;
		}

		public PrivateUserInfo(uint _userID, string _prefix, string _firstName, string _middleName, 
								string _lastName, string _suffix, string _streetAddress, string _city,
								uint _zipCode, string _state, string _country)
		{
			userID = _userID;
			prefix = _prefix;
			firstName = _firstName;
			middleName = _middleName;
			lastName = _lastName;
			suffix = _suffix;
			streetAddress = _streetAddress;
			city = _city;
			zipCode = _zipCode;
			state = _state;
			country = _country;
		}

		//Deserializer
		public PrivateUserInfo(SerializationInfo info, StreamingContext ctxt)
		{
			userID = info.GetUInt32("userID");
			prefix = info.GetString("prefix");
			firstName = info.GetString("firstName");
			middleName = info.GetString("middleName");
			lastName = info.GetString("lastName");
			suffix = info.GetString("suffix");
			streetAddress = info.GetString("streetAddress");
			city = info.GetString("city");
			zipCode = info.GetUInt32("zipCode");
			state = info.GetString("state");
			country = info.GetString("country");
		}

		//Serializer
		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("userID", userID);
			info.AddValue("prefix", prefix);
			info.AddValue("firstName", firstName);
			info.AddValue("middleName", middleName);
			info.AddValue("lastName", lastName);
			info.AddValue("suffix", suffix);
			info.AddValue("streetAddress", streetAddress);
			info.AddValue("city", city);
			info.AddValue("zipCode", zipCode);
			info.AddValue("state", state);
			info.AddValue("country", country);
		}

		public string getRealName()
		{
			return prefix + " " + firstName + " " + middleName + " " + lastName + " " + suffix;
		}
	}
}
