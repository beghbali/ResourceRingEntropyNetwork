using System;
using System.Collections;

namespace RRLib
{
	/// <summary>
	/// A Threaded Binary Tree library. NOT THREAD SAFE!!!
	/// </summary>
	public class BinaryTree : IEnumerable
	{
		public class BTNode
		{
			public IComparable data;
			public BTNode left;
			public BTNode right;
			public BTNode parent;
			public BTNode next;

			public BTNode(IComparable _data)
			{
				data = _data;
				left = null;
				right = null;
				parent = null;
				next = null;
			}
		}

		public class BTEnumerator : IEnumerator
		{
			public BTNode current;
			public BTNode btroot;

			public BTEnumerator(BTNode _btroot)
			{
				btroot = _btroot;
				current = null;
			}

			public bool MoveNext ()
			{
				if (current == null)
					current = btroot;
				else
					current = current.next;
				
				if (current == null)
					return false;
				return true;
			}

			public void Reset()
			{
				current = null;
			}
			
			public object Current
			{
				get { return current.data; }
			}
		}

		private BTNode root;
		private ArrayList threads;
		private int count;

		public BinaryTree()
		{
			root = null;
			threads = new ArrayList(4);
		}

		private BTNode findSmallestNode(BTNode base_node)
		{
			BTNode mover = base_node;
			while (base_node.left != null)
				mover = base_node.left;
			return mover;
		}

		private BTNode findNextLargestNode(BTNode base_node)
		{
			BTNode mover = base_node;
			while (mover.parent != null && mover.parent.right == mover)
				mover = mover.parent;

			// reached root, base_node must be largest in the tree
			if (mover.parent == null)
				return null;

			if (mover.parent.left == mover)
				return mover;
			//should never get here
			return null;
		}

		public void Add(object data)
		{
			if (!(data is IComparable))
				return;
			root = add_internal(root, (IComparable)data);
			count++;
		}

		private BTNode add_internal(BTNode base_node, IComparable data)
		{
			if (base_node == null)
				return(new BTNode(data));

			BTNode node = base_node;
			bool inserted = false;
			//do it iteratively. Recursion is performance killer
			while (!inserted)
			{
				if (node.data.CompareTo(data) < 0)
				{
					if (node.left == null)
					{
						node.left = new BTNode(data);
						node.left.parent = node;
						node.left.next = node;
						inserted = true;
					}
					else
						node = node.left;
				}
				else if (node.data.CompareTo(data) > 0)
				{
					if (node.right == null)
					{
						node.right = new BTNode(data);
						node.right.parent = node;
						node.right.next = findNextLargestNode(node);
						inserted = true;
					}
					else
						node = node.right;
				}
				else  //equal to insert as smallest in the right subtree
				{
					node.right = add_internal(node.right, data);
					// for the case when right node is null
					if (node.right.next == null)
					{
						node.right.parent = node;
						node.right.next = findNextLargestNode(node.right);
					}
					inserted = true;
				}
			}

			return base_node;
		}

		public void Clear()
		{
			root = null;
		}

		private BTNode find(IComparable key)
		{
			BTNode node = root;
			while (node != null)
			{
				if (node.data.CompareTo(key) == 0)
					return node;
				else if(node.data.CompareTo(key) > 0)
					node = node.left;
				else
					node = node.right;
			}
			return null;
		}

		public bool Contains(object key)
		{
			return (find((IComparable)key) != null);
		}

		public void Remove(object key)
		{
		}

		public int Count
		{
			get { return count; }
		}

		public IEnumerator GetEnumerator()
		{
			BTEnumerator newEnumerator = new BTEnumerator(root);
			threads.Add(newEnumerator );
			return newEnumerator;
		}
	}
}
