using System;
using System.Collections;

namespace RRLib
{
	/// <summary>
	/// Summary description for QueryProcessor.
	/// </summary>
	public class QueryProcessor
	{
		private Hashtable stopWordsDB;
		private static QueryProcessor instance;

		public QueryProcessor(string[] stopWords)
		{
			stopWordsDB = new Hashtable();
			foreach (string stopWord in stopWords)
				stopWordsDB.Add(stopWord, stopWord);
		}

		public static QueryProcessor getInstance()
		{
			if (instance == null)
				instance = new QueryProcessor(Constants.STOPWORDS);
			return instance;
		}

		public string[] queryFromQueryString(string queryString)
		{
			string[] tokenizedQuery;
			queryString = queryString.ToLower();
			tokenizedQuery = queryString.Split(Constants.TOKENIZER_DELIMETERS);
			
			int numKeywords = tokenizedQuery.Length;
			for(uint index = 0; index < tokenizedQuery.Length; index++)
			{
				if(stopWordsDB.Contains(tokenizedQuery[index]))
				{
					tokenizedQuery[index] = null;
					numKeywords--;
				}
			}
			
			string[] query = new string[numKeywords];
			uint addIndex = 0;
			foreach (string keyword in tokenizedQuery)
			{
				if(keyword != null)
					query[addIndex++] = keyword;
			}
			return query;
		}

		public float calculateSimilarity(string keyword1, string keyword2)
		{
			//REVISIT: make sure at this point both keyword1 and keyword2 are stemmed
			int compare = keyword1.CompareTo(keyword2);
			return compare == 0 ? 1.0f : 0.0f;
		}

		public float calculatePartialResourceWeight(string keyword, string[] text)
		{
			uint fkd = 0;

			foreach (string textKeyword in text)
			{
				if (keyword.CompareTo(textKeyword) == 0)
					fkd++;
			}

			return ((float)fkd/text.Length);
		}
	}
}
