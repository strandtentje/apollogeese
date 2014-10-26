using System;
using System.Collections.Generic;
using BorrehSoft.Utensils;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps.Search;

namespace BorrehSoft.Extensions.BasicWeblings.Lookup
{
	/// <summary>
	/// A lookup for lookups. Static. Because I like to see the world burn beneath me.
	/// </summary>
	public static class Lookups
	{
		static private Dictionary<string, SearchMap<LookupEntry>> lookupLookup = new Dictionary<string, SearchMap<LookupEntry>>();

		public static SearchMap<LookupEntry> Get (string LookupName)
		{
			SearchMap<LookupEntry> thisLookup;

			if (lookupLookup.ContainsKey (LookupName))
				thisLookup = lookupLookup [LookupName];
			else {
				thisLookup = new SearchMap<LookupEntry>();
				lookupLookup.Add (LookupName, thisLookup);
			}

			return thisLookup;
		}
		
		public static IEnumerable<string> GetKeylist (object data, int maxLength = int.MaxValue)
		{			
			List<string> suppliedKeys = new List<string> ();

			if (data is String) 
				suppliedKeys.Add (data as String);


			if (data is IEnumerable<string>) {
				IEnumerable<string> kwSource = data as IEnumerable<string>;
				int itemCount = 0;

				foreach(string keyword in kwSource) {
					if (maxLength > itemCount)
					{
						suppliedKeys.Add(keyword);
						itemCount++;
					} 
					else 
					{
						break;
					}
				}
			}	

			return suppliedKeys;
		}

	}
}

