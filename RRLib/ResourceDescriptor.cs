using System;

namespace RRLib
{
	/// <summary>
	/// Used for inserting resource/file information in the inverted keyword index.
	/// </summary>
	public class ResourceDescriptor : IComparable
	{
		private Resource _resource;
		public Resource resource
		{
			get { return _resource; }
			set { _resource = value; }
		}

		private float _weight;
		public float weight
		{
			get { return _weight; }
			set { _weight = value; }
		}

		private uint _frequencyOfKeyword;
		public uint frequencyOfKeyword
		{
			get { return _frequencyOfKeyword; }
			set { _frequencyOfKeyword = value; }
		}

		private uint _numKeywords;
		public uint numKeywords
		{
			get { return _numKeywords; }
			set { _numKeywords = value; }
		}

		public ResourceDescriptor(Resource __resource, float __weight, uint __frequency, uint __numKeywords)
		{
			resource = __resource;
			weight = __weight;
			frequencyOfKeyword = __frequency;
			numKeywords = __numKeywords;
		}

		public int CompareTo(object other)
		{
			if (weight < ((ResourceDescriptor)other).weight)
				return -1;
			if (weight > ((ResourceDescriptor)other).weight)
				return 1;
				
			return 0;
		}
	}
}
