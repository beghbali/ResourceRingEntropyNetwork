using System;
using System.Collections;

namespace RRLib
{
	/// <summary>
	/// Summary description for ResourceGroup.
	/// </summary>
	public class ResourceGroup
	{
		private string _groupName;
		public string groupName
		{
			get { return _groupName; }
			set { _groupName = value; }
		}

		private Hashtable _resourceList;
		public Hashtable resourceList
		{
			get { return _resourceList; }
			set { _resourceList = value; }
		}

		public ResourceGroup(string __groupName)
		{
			groupName = __groupName;
			resourceList = new Hashtable();
		}

		public ResourceGroup(string __groupName, Resource[] __resources)
		{
			groupName = __groupName;
			resourceList = new Hashtable();
			addResourcesToGroup(__resources);
		}

		public void addResourceToGroup(Resource newResource)
		{
			resourceList[newResource.header.resourceID] = newResource;
		}

		public void addResourcesToGroup(Resource[] newResources)
		{
			foreach (Resource newResource in newResources)
			{
				resourceList[newResource.header.resourceID] = newResource;
			}
		}

		public virtual bool resourceBelongsToThisGroup(Resource resource)
		{
			return false;	
		}
	}
}
