using System;

namespace RRLib
{
	/// <summary>
	/// Summary description for Ring.
	/// </summary>
	public class Ring
	{
		private uint _ringID;
		public uint ringID
		{
			get { return _ringID; }
			set { _ringID = value; }
		}

		private string _ringName;
		public string ringName
		{
			get { return _ringName; }
			set { _ringName = value; }
		}

		private IPEndPoint[] _lords;
		public IPEndPoint[] lords
		{
			get { return _lords; }
			set { _lords = value; }
		}

		private string[] _extensions;
		public string[] extensions
		{
			get { return _extensions; }
			set { _extensions = value; }
		}

		private ResourceGroup[] _group;
		public ResourceGroup[] group
		{
			get { return _group; }
			set {_group = value; }
		}

		public Ring(uint __ringID, string __ringName, string[] __extensions, ResourceGroup[] __group)
		{
			ringID = __ringID;
			ringName = __ringName;
			extensions = __extensions;
			group = __group;
		}

		public Ring(uint __ringID, string __ringName, IPEndPoint[] __lords)
		{
			ringID = __ringID;
			ringName = __ringName;
			lords = __lords;
		}
	}
}
