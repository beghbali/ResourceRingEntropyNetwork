using System;

namespace RRLib
{
	/// <summary>
	/// For search match results
	/// </summary>
	public class ResourceMatch : IComparable
	{
		private ResourceDescriptor _rd;
		public ResourceDescriptor rd
		{
			get { return _rd; }
			set { _rd = value; }
		}

		private float _numerator;
		public float numerator
		{
			get { return _numerator; }
			set { _numerator = value; }
		}

		private float _resourceLength;
		public float resourceLength
		{
			get { return _resourceLength; }
			set { _resourceLength = value; }
		}

		private float _queryLength;
		public float queryLength
		{
			get { return _queryLength; }
			set { _queryLength = value; }
		}

		public ResourceMatch(ResourceDescriptor __rd, float __numerator, float __resourceLength, float __queryLength)
		{
			rd = __rd;
			numerator = __numerator;
			resourceLength = __resourceLength;
			queryLength = __queryLength;
		}

		public int CompareTo(object other)
		{
			if (numerator < ((ResourceMatch)other).numerator)
				return -1;
			if (numerator > ((ResourceMatch)other).numerator)
				return 1;
				
			return 0;
		}
	}
}
