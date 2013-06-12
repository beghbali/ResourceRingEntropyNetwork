using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;

namespace RRLib
{
	/// <summary>
	/// General utility library
	/// </summary>
	public abstract class LibUtil
	{
		public static int countValidElements(string[] array)
		{
			int valid = 0;
			foreach (string s in array)
			{
				if (s != null && s != "")
					valid++;
			}
			return valid;
		}

		public static string[] uniquify(string[] array)
		{
			int index = 0;
			string[] retArray;

			foreach (string element in array)
			{
				if (element == "")
				{
					index++;
					continue;
				}

				for (int mover = index+1; mover < array.Length; mover++)
				{
					//remove recurrences
					if (array[mover] == element)
						array[mover] = null;
				}
				index++;
			}

			retArray = new string[countValidElements(array)];
			index = 0;
			foreach (string element in array)
			{
				if (element != null && element != "")
					retArray[index++] = element;
			}

			return retArray;
		}

		public static string detokenize(string[] tokens, char delimeter)
		{
			string retval = "";
			if(tokens.Length == 0)
				return null;
			foreach (string token in tokens)
			{
				retval += token + delimeter;
			}
			retval.Remove(retval.Length-1,1);
			return retval;
		}

		public static string[] InformationEntropyToStringArray(InformationEntropy[] IEList)
		{
			string[] array = new string[IEList.Length];
			int index = 0;

			foreach (InformationEntropy IE in IEList)
			{
				array[index++] = IE.keyword;
			}

			return array;
		}

		public static string formatSize(long size)
		{
			double ter, gig, meg, kil;

			if(size > Constants.TERABYTE)
			{
				ter = (double)size / Constants.TERABYTE;
				return (ter + "TBytes(" + size + ")");
				}
			else if (size > Constants.GIGABYTE)
			{
				gig = (double)size / Constants.GIGABYTE;
				return (gig + "GBytes(" + size + ")");
				}
			else if (size > Constants.MEGABYTE)
			{
				meg = (double)size / Constants.MEGABYTE;
				return (meg + "MBytes(" + size + ")");
				}
			else if (size > Constants.KILOBYTE)
			{
				kil = (double)size / Constants.KILOBYTE;
				return (kil + "KBytes(" + size + ")");
				}
			
			return size + "Bytes";
		}

		public static bool ShiftArrayItems(object[] array, int startIndex, int numToShift, int shiftAmount)
		{
			int endIndex = (startIndex-1 + numToShift);
			//not enough room to shift
			if (array.Length - endIndex - shiftAmount < 0)
				return false;
		
			for (int src = endIndex, dest = endIndex + shiftAmount; src >= startIndex && dest < array.Length; src--, dest--)
			{
				array[dest] = array[src];
				array[src] = null;
			}

			return true;																							   
		}

		public static bool ShiftListItems(IList array, int startIndex, int numToShift, int shiftAmount)
		{
			int endIndex = (startIndex-1 + numToShift);
				
			for (int src = endIndex, dest = endIndex + shiftAmount; src >= startIndex ; src--, dest--)
			{
				array.Insert(dest, array[src]);
				array[src] = null;
			}

			return true;																							   
		}

		public static byte[] SliceArray(byte[] array, long startIndex, long sliceSize)
		{
			long endIndex = startIndex + sliceSize - 1;

			if (endIndex >= array.Length)
				endIndex = array.Length -1;

			byte[] ret_array = new byte[endIndex - startIndex + 1];

			for (long destIndex = 0; startIndex <= endIndex; startIndex++, destIndex++) 
			{
				ret_array[destIndex] = array[startIndex];
			}

			return ret_array;
		}
	
		public static bool QueriesEqual(string[] query1, string[] query2)
		{
			if (query1.Length != query2.Length)
				return false;

			for (int index = 0; index < query1.Length; index++)
			{
				if (query1[index] != query2[index])
					return false;
			}
			return true;
		}
	}

}
