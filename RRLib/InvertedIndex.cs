using System;
using System.IO;
using System.Collections;
using System.Diagnostics;

namespace RRLib
{
	/// <summary>
	/// Takes care of building and maintaining an inverted index
	/// </summary>
	public class InvertedIndex
	{
		#region private variables

		private Hashtable index;

		#endregion

		#region constructors

		public InvertedIndex()
		{
			index = new Hashtable();	
		}
		#endregion

		#region member methods

		public void add(object _key, object _value)
		{
			index.Add(_key, _value);
		}

		public bool isEmpty()
		{
			return (index.Count == 0);
		}

		public object getInvertedList(object key)
		{
			return index[key];
		}

		public IDictionaryEnumerator getEnumerator()
		{
			return index.GetEnumerator();
		}
		#endregion

		#region properties

		public int Count
		{
			get { return index.Count; }
		}

		#endregion

		#region static methods

		private static InformationEntropy[] extractInformationEntropy_internal(string text)
		{
			//REVISIT: reduce the # copies
			string[] tokens = text.Split(Constants.TOKENIZER_DELIMETERS);
			//add stopword check
			Hashtable keywords = new Hashtable();
			int frequency;
			object freqObj;
			string keyword;
			foreach (string newKeyword in tokens)
			{
				keyword = newKeyword.ToLower();
				if (keyword.Equals(""))
					continue;

				freqObj = keywords[keyword];
				if (freqObj == null)
					keywords[keyword] = 1;
				else
				{
					frequency = ((int)freqObj);
					keywords[keyword] = ++frequency;
				}
			}
			
			IDictionaryEnumerator enumerator = keywords.GetEnumerator();
			InformationEntropy[] ret_IE = new InformationEntropy[keywords.Count];
			int insertIndex = 0;
			while (enumerator.MoveNext())
			{
				ret_IE[insertIndex++] = new InformationEntropy((string)enumerator.Key, (float)((int)enumerator.Value));
			}
			
			return ret_IE;
		}

		public static InformationEntropy[] extractInformationEntropy(string text)
		{
			return (extractInformationEntropy_internal(text));	
		}

		public static InformationEntropy[] extractInformationEntropy(FileInfo file)
		{
			string text = FileLib.readFile(file);
			if (text == null)
				return null;

			return (extractInformationEntropy_internal(text));	
		}

		#endregion
	}
}
