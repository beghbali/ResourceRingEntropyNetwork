using System;
using System.Collections;
using System.IO;

namespace RRLib
{
	/// <summary>
	/// Summary description for FileGroup.
	/// </summary>
	public class FileGroup : ResourceGroup
	{
		private string[] _filters;
		public string[] filters
		{
			get { return _filters; }
			set { _filters = value; }
		}

		public FileGroup(string groupName, string[] __filters) : base(groupName)
		{
			filters = __filters;
		}

		public FileGroup(string groupName, Resource[] files, string[] __filters) : base(groupName, files)
		{
			filters = __filters;
		}

		public FileInfo[] filterFiles(FileInfo[] files)
		{
			ArrayList filteredFiles = new ArrayList();
			foreach (FileInfo file in files)
			{
				foreach (string filter in filters)
				{
					if (file.Extension == filter)
					{
						filteredFiles.Add(file);
						break;
					}
				}
			}
			return (FileInfo[])filteredFiles.ToArray(typeof(FileInfo));
		}

		public override bool resourceBelongsToThisGroup(Resource resource)
		{
			string extention = FileLib.getFileExtention(resource);
			foreach (string filterExtention in this.filters)
			{
				if (filterExtention.CompareTo(extention) == 0)
					return true;
			}
			return false;
		}
	}
}
