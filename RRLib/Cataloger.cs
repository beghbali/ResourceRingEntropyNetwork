using System;
using System.IO;
using System.Collections;

namespace RRLib
{
	public delegate void setupUIforCatalog(int min, int max);
	public delegate void updateUIforCatalog(int amount);

	/// <summary>
	/// Summary description for Cataloger.
	/// </summary>
	public class Cataloger
	{
		private ArrayList _roots;   //DirectoryInfo
		public ArrayList roots
		{
			get { return _roots; }
			set { _roots = value; }
		}

		private ArrayList _groups;    //ResourceGroup
		public ArrayList groups
		{
			get { return _groups; }
			set { _groups = value; }
		}

		private InvertedIndex _resourcesIndex;
		public InvertedIndex resourcesIndex
		{
			get { return _resourcesIndex; }
			set { _resourcesIndex = value; }
		}

		private InformationEntropy[] _calculatedIE;
		public InformationEntropy[] calculatedIE
		{
			get { return _calculatedIE; }
			set { _calculatedIE = value; }
		}

		private setupUIforCatalog setupUI;
		private updateUIforCatalog updateUI;

		public Cataloger(setupUIforCatalog _setupUI, updateUIforCatalog _updateUI)
		{
			roots = new ArrayList();
			groups = new ArrayList();
			setupUI = _setupUI;
			updateUI = _updateUI;
			resourcesIndex = new InvertedIndex();
		}

		public void catalog()
		{
			int numFiles = 0;
			foreach (DirectoryInfo directory in roots)
				numFiles += FileLib.fileCountRecursive(directory);

			setupUI(0, numFiles);
			catalog_internal(roots);
		}

		private void catalog_internal(ICollection directories)
		{
			FileInfo[] files;
			FileInfo[] filteredFiles = null;
			Resource[] newResources;

			foreach (DirectoryInfo directory in directories)
			{
				files = directory.GetFiles();
				
				foreach (ResourceGroup group in groups)
				{
					if (group == null)
						break;
					if (!(group is FileGroup))
						continue;
					filteredFiles = ((FileGroup)group).filterFiles(files);
					newResources = Resource.filesToResources(filteredFiles);
					addResources(newResources);
					group.addResourcesToGroup(newResources);
				}
				updateUI(files.Length-filteredFiles.Length);
				//recursively process subdirectories
				catalog_internal(directory.GetDirectories());
			}
			finalizeResourcesIndex();
		}

		public void addResources(Resource[] resources)
		{
			QueryProcessor queryProcessor = QueryProcessor.getInstance();
			float partialWeight;  /* 
								   * This is partial because only after all resources have been inserted we can update the 
								   * second part of the weight formula N/dfk
								   */
			try
			{
				foreach (Resource resource in resources)
				{
					foreach (InformationEntropy IE in resource.IE)
					{
						string keyword = IE.keyword;

						//note that IE.weight contains the frequency of keyword in resource (FDk)
						partialWeight = IE.weight/resource.IE.Length;
						RRLib.SortedList invertedList = (RRLib.SortedList)resourcesIndex.getInvertedList(keyword);
						if (invertedList == null)
						{
							invertedList = new RRLib.SortedList();
							resourcesIndex.add(keyword, invertedList);
						}
				
						invertedList.Add(new ResourceDescriptor(resource, partialWeight, 
							(uint)(partialWeight*resource.IE.Length), (uint)resource.IE.Length));
					}
					updateUI(1);
				} 
			}
			catch (Exception e)
			{
				int x = 2;
			}
		}

		private void finalizeResourcesIndex()
		{
			IDictionaryEnumerator enumerator = resourcesIndex.getEnumerator();
			calculatedIE = new InformationEntropy[resourcesIndex.Count > Constants.MAX_INFORMATION_ENTROPY_LENGTH ?
				Constants.MAX_INFORMATION_ENTROPY_LENGTH : resourcesIndex.Count];
			
			int IECounter = 0;
					
			while(enumerator.MoveNext())
			{
				RRLib.SortedList invertedList = (RRLib.SortedList)enumerator.Value;
				int dfk = invertedList.Count;
				int N = resourcesIndex.Count;
				float IDFk = (float)Math.Log(N/dfk, 10);
				foreach (ResourceDescriptor rd in invertedList)
				{
					rd.weight *= IDFk;   //finish the weight calculation.
				}
				//Calculate our Information Entropy (top keywords)
				Accumulate(calculatedIE, IECounter++, IDFk, (string)enumerator.Key); 
			}
		}
		/// <summary>
		/// Accumulates the top keywords (those with most frequency) to establish as our ring's IE.
		/// Uses the Binary Insertion Algorithm
		/// </summary>
		/// <param name="IEList"></param>
		/// <param name="frequency"></param>
		/// <param name="keyword"></param>
		private void Accumulate (InformationEntropy [] IEList, int position, float weight, string keyword)
		{
			InformationEntropy IE = new InformationEntropy(keyword, weight);
			bool done = false;
			int index = position;
			int endIndex = position;
			int beginIndex = 0;
			int comparison = -1;
			bool evict = false;
			bool headInsert = false;

			if (position >= IEList.Length)
			{
				endIndex = IEList.Length;
				position = IEList.Length;
				evict = true;
			}

			while(!done)
			{
				//only if we are full calculate comparison index and do actual comparing
				if (evict)
				{
					index = beginIndex + (endIndex - beginIndex)/2;
					comparison = IE.CompareTo(IEList[index]);
				}
				else
					headInsert = true;	

				if (comparison == 0 || headInsert)
				{
					if (!evict && IEList[index] != null)
						LibUtil.ShiftArrayItems(IEList, index, position -index, 1);
					IEList[index] = IE;
					
					done = true;
				}
				else if (comparison < 0)
				{
					//if less than the last element just ignore it
					if (index == IEList.Length-1)
						done = true;
					else
						beginIndex = index;
				}
				else if (comparison > 0)
				{ 
					//if greater than the first element just ignore it
					if (index == 0)
						headInsert = true;
					else
						endIndex = index;
				}			
			}
		}

		public ResourceGroup GetResourceGroup (Resource resource)
		{
			foreach (ResourceGroup group in this.groups)
			{
				if (group.resourceBelongsToThisGroup(resource))
					return group;
			}
			return null;
		}
	}
}
