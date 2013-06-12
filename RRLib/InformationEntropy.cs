using System;
using System.Runtime.Serialization;

namespace RRLib
{
	/// <summary>
	/// Summary description for InformationEntropy.
	/// </summary>
	[Serializable]
	public class InformationEntropy : ISerializable, IComparable
	{
		private string _keyword;
		public string keyword
		{
			get { return _keyword; }
			set { _keyword = value; }
		}

		private float _weight;
		public float weight
		{
			get { return _weight; }
			set { _weight = value; }
		}

		public InformationEntropy(string __keyword, float __weight)
		{
			keyword = __keyword;
			weight = __weight;
		}

		//Deserializer
		public InformationEntropy(SerializationInfo info, StreamingContext ctxt)
		{
			keyword = info.GetString("keyword");
			weight = (float)info.GetValue("weight", typeof(float));
		}

		//Serializer
		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("keyword", keyword);
			info.AddValue("weight", weight);
		}
		
		public int CompareTo(object other)
		{
			if (weight < ((InformationEntropy)other).weight)
				return -1;
			if (weight > ((InformationEntropy)other).weight)
				return 1;
				
			return 0;
		}

		public static string Detokenize (InformationEntropy[] IEList, char delimeter)
		{
			string IEString = "";
			foreach (InformationEntropy IE in IEList)
			{
				IEString += delimeter + IE.keyword;
			}
			return IEString;
		}
	}
}
