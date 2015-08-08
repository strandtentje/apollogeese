using System;
using System.Collections.Generic;
using BorrehSoft.Utensils;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps.Search;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Lookup
{
	/// <summary>
	/// A lookup for lookups. Static. Because I like to see the world burn beneath me.
	/// </summary>
	public static class Lookups
	{
		/// <summary>
		/// The lookup lookup.
		/// </summary>
		static private Dictionary<string, SearchMap<LookupEntry>> lookupLookup = 
			new Dictionary<string, SearchMap<LookupEntry>>();

		/// <summary>
		/// Drops a lookup by name.
		/// </summary>
		/// <param name="lookupName">Lookup name.</param>
		public static void DropLookup(string lookupName) {
			if (lookupLookup.ContainsKey(lookupName)) lookupLookup.Remove (lookupName);
		}

		/// <summary>
		/// Gets a lookup by name
		/// </summary>
		/// <param name="LookupName">Lookup name.</param>
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

		/// <summary>
		/// Turns a source of keywords in another source of keywords. I seriously question if there's any
		/// point in having this around or using this, but I'm currently not interresting in maintaining
		/// these modules too much. So let's keep it.
		/// </summary>
		/// <returns>The keylist.</returns>
		/// <param name="kwSource">Kw source.</param>
		/// <param name="maxLength">Max length.</param>
		public static IEnumerable<string> GetKeylist (IEnumerable<string> kwSource, int maxLength = int.MaxValue)
		{		
			List<string> suppliedKeys = new List<string>();

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

			return suppliedKeys;
		}

	}
}

