using System;
using System.Collections;

namespace RRLib
{
	/// <summary>
	/// Summary description for SortedList.
	/// </summary>
	public class SortedList : CollectionBase, IEnumerable, IList
	{
		//private BinaryTree list;
		//private System.Collections.SortedList list;
		private SortedArrayList list;

		public SortedList()
		{
			//list = new BinaryTree();	
			//list = new System.Collections.SortedList();
			list = new SortedArrayList();
		}

		public int Add(object data)
		{
			//list.Add(data, data);
			return list.Add(data);
		}

		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public void RemoveAt(int index)
		{
			list.RemoveAt(index);
		}

		public void Remove(object data)
		{
			list.Remove(data);
		}

		public void Insert (int index, object data)
		{
			list.Insert(index, data);
		}

		public int IndexOf(object data)
		{
			return list.IndexOf(data);
		}

		public bool Contains(object data)
		{
			return list.Contains(data);
		}

		public void Clear()
		{
			list.Clear();
		}

		public int Count
		{
			get { return list.Count; }
		}

		public bool IsReadOnly
		{
			get { return list.IsReadOnly; }
		}

		public bool IsFixedSize
		{
			get { return list.IsFixedSize; }
		}

		public object this[int index]
		{
			get { return list[index]; }
			set { list[index] = value; }
		}
	}
}
