using System;
using System.Collections;

namespace RRLib
{
	/// <summary>
	/// Summary description for RelevanceRanking.
	/// </summary>
	public class RelevanceRanking
	{
		private QueryProcessor queryProcessor;

		private static RelevanceRanking instance;

		public RelevanceRanking(QueryProcessor __queryProcessor)
		{
			queryProcessor = __queryProcessor;
		}
		
		public static RelevanceRanking getInstance()
		{
			if (instance == null)
				instance = new RelevanceRanking(QueryProcessor.getInstance());
			return instance;
		}

		public float calculateScore(string[] query, InformationEntropy[] IEs)
		{
			float score = 0.0f;

			foreach (String keyword in query)
			{
				foreach (InformationEntropy IE in IEs)
				{
					score += queryProcessor.calculateSimilarity(keyword, IE.keyword);
				}
			}
			//normalize to the query length
			return (score/ query.Length);
		}

		public Resource[] rankDatabase(InvertedIndex db, string[] query, string[] columns)
		{
			RRLib.SortedList resources;
			float Wkd, Wkq = 1.0f;
			float existingNumerator, existingResourceLength, existingQueryLength;
			Resource resource;
			ResourceMatch existingMatch;
			Hashtable results = new Hashtable(Constants.MAX_SEARCH_RESULTS_FROM_PEER, 0.9f);

			foreach (string keyword in query)
			{
				if ((resources = (RRLib.SortedList)db.getInvertedList(keyword)) != null)
				{
					int resourceToConsider = Constants.MAX_RESOURCES_TO_CONSIDER_FOR_KEYWORD-1 > resources.Count ?  resources.Count-1 : Constants.MAX_RESOURCES_TO_CONSIDER_FOR_KEYWORD-1;
					for (;resourceToConsider >= 0; resourceToConsider--)
					{
						ResourceDescriptor rd = (ResourceDescriptor)resources[resourceToConsider];
						resource = rd.resource;
						Wkd = rd.weight;
						if ((existingMatch = (ResourceMatch)results[resource.header.resourceID]) != null)
						{
							existingMatch.numerator = existingMatch.numerator +(Wkd*Wkq); 
							existingMatch.resourceLength = existingMatch.resourceLength + (float)Math.Pow(Wkd, 2.0);
							existingMatch.queryLength  = existingMatch.queryLength + (float)Math.Pow(Wkq, 2.0);

						}
						else
						{
							existingNumerator = (Wkd*Wkq); 
							existingResourceLength = (float)Math.Pow(Wkd, 2);
							existingQueryLength = (float)Math.Pow(Wkq, 2);

							results[rd.resource.header.resourceID] = new ResourceMatch(rd, existingNumerator, 
								existingResourceLength, existingQueryLength); 
						}
					}
				}
			}
			ResourceMatch[] ret_results = new ResourceMatch[results.Count];
			int insertIndex = 0;
			IDictionaryEnumerator enumerator = results.GetEnumerator();
			float score;
			ResourceMatch match;
			while (enumerator.MoveNext())
			{
				match = (ResourceMatch)enumerator.Value;

				//now the numerator contains the commulative score
				match.numerator = match.numerator/(float)(Math.Sqrt(match.resourceLength) * Math.Sqrt(match.queryLength));
				ret_results[insertIndex++] = match;
			}

			Array.Sort(ret_results, 0, insertIndex);

			//Remove any extra matches beyond the max acceptable
			int clearRange = Math.Min(insertIndex, Constants.MAX_QUERY_MATCHES);
			Array.Clear(ret_results, clearRange, results.Count- clearRange);
			Resource[] final_results = new Resource[ret_results.Length];
			insertIndex = 0;
			foreach (ResourceMatch rMatch in ret_results)
			{
				final_results[insertIndex++] = rMatch.rd.resource;
			}

			return  final_results;
		}
	}
}
