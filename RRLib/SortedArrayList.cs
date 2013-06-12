using System;
using System.Collections;

namespace RRLib
{
	/// <summary>
	/// Summary description for SortedArrayList.
	/// </summary>
	public class SortedArrayList : CollectionBase, IList
	{
		private ArrayList list;

		public SortedArrayList()
		{
			list = new ArrayList();
		}

		public int Count
		{
			get { return list.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool IsFixedSize
		{
			get { return false; }
		}

		public object this[int index]
		{
			get { return list[index]; }
			set { list[index] = value; }
		}

		public void Clear()
		{
			list.Clear();
		}

		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public void RemoveAt(int index)
		{
			list.RemoveAt(index);
		}

		public void Remove (object data)
		{
			list.Remove(data);
		}

		public void Insert (int index, object data)
		{
			list.Insert(index, data);
		}

		public int IndexOf (object data)
		{
			return list.IndexOf(data);
		}

		public bool Contains(object data)
		{
			return list.Contains(data);
		}

		public int Add(object dataObj)
		{
			if (!(dataObj is IComparable))
				return -1;

			if (list.Count == 0)
				return list.Add(dataObj);

			IComparable data = (IComparable)dataObj;
			int beginIndex = 0;
			int endIndex = list.Count-1;
			bool inserted = false;
			int index = -1, comparison;

			while (!inserted)
			{
				index = beginIndex + (endIndex+1-beginIndex)/2;

				comparison = data.CompareTo((IComparable)list[index]);
			
				if (comparison > 0)
				{
					beginIndex = index;
				}
				else if (comparison < 0)
				{
					endIndex = index;
				}
				if (comparison == 0 || endIndex - beginIndex <= 1 )
				{
					//LibUtil.ShiftListItems(list, index, list.Count - index, 1);
					list.Insert(index, data);
					inserted = true;
				}
			}

			return index;
		}
	}
}
