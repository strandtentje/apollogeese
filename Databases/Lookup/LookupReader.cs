using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections.Maps.Search;
using BorrehSoft.Utensils.Collections;
using System.Text.RegularExpressions;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Lookup
{
	/// <summary>
	/// Reads one or multiple lookup entries for given keywords
	/// </summary>
	public class LookupReader : Service
	{
		private Service iterator;
		private Service capreached = Stub;
		/// <summary>
		/// Gets or sets the name at which the lookup search query should reside within interactions
		/// </summary>
		/// <value>The name of the lookup key.</value>
		private string LookupKeyName { get; set; }
		/// <summary>
		/// Gets or sets the name of the lookup.
		/// </summary>
		/// <value>The name of the lookup.</value>
		private string LookupName {	get; set; }
		/// <summary>
		/// Gets or sets the maximum amount of allowed query words
		/// </summary>
		/// <value>The key cap.</value>
		private int KeyCap { get; set; }
		/// <summary>
		/// Gets or sets the maximum amount of allowed results
		/// </summary>
		/// <value>The result cap.</value>
		private int ResultCap { get; set; }
		/// <summary>
		/// Gets or sets the keyword splitter used to turn a query string into seperate query words.
		/// </summary>
		/// <value>The keyword splitter.</value>
		private Regex KeywordSplitter { get; set; }

		public override string Description {
			get {
				return this.LookupName;
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "iterator") 
				this.iterator = e.NewValue;
			if (e.Name == "capreached")
				this.capreached = e.NewValue;
		}

		protected override void Initialize (Settings modSettings)
		{
			LookupKeyName = modSettings ["lookupkeyname"] as String;
			LookupName = modSettings ["lookupname"] as String;
			KeywordSplitter = new Regex(modSettings.GetString("keywordsplitregex", @"\W|_"));
			KeyCap = modSettings.GetInt("keycap", 6);
			ResultCap = modSettings.GetInt ("resultcap", -1);
		}

		protected override bool Process (IInteraction parameters)
		{
			string queryText; int resultCount = 0;
			bool success = parameters.TryGetFallbackString (this.LookupKeyName, out queryText);

			if (success) {
				IEnumerable<string> keylist = Lookups.GetKeylist (KeywordSplitter.Split(queryText), KeyCap);
				CleverSet<LookupEntry> results =  Lookups.Get(LookupName).Find (keylist);

				foreach (LookupEntry result in results.Values) {
					if ((ResultCap != -1) && (resultCount++ >= ResultCap)) {
						this.capreached.TryProcess (parameters);
						break;
					} else {
						this.iterator.TryProcess (result.Parameters.Clone (parameters));
					}
				} 
			}

			return success;
		}
	}
}

