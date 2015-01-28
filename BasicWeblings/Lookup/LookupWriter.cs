using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections.Maps.Search;
using System.Text.RegularExpressions;

namespace BorrehSoft.Extensions.BasicWeblings.Lookup
{
	public class LookupWriter : Service
	{
		private string LookupKeyName { get; set; }
		private string LookupName {	get; set; }
		private string MetaName { get; set; }
		private Regex KeywordSplitter { get; set; }
		private bool SplitKeywords { get; set; }
		private bool LookupMaster { get; set; }

		private SearchMap<LookupEntry> thisLookup;

		public override string Description {
			get {
				return this.LookupName + " by " + this.MetaName;
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}	

		protected override void Initialize (Settings modSettings)
		{
			LookupKeyName = modSettings ["lookupkeyname"] as String;
			LookupName = modSettings ["lookupname"] as String;
			MetaName = modSettings ["metaname"] as String;
			KeywordSplitter = new Regex(modSettings.GetString("keywordsplitregex", @"\W|_"));
			SplitKeywords = modSettings.GetBool ("splitkeywords", true);

			Lookups.DropLookup (LookupName);
			thisLookup = Lookups.Get(LookupName);
		}

		protected override bool Process (IInteraction parameters)
		{
			IEnumerable<string> keywords;
			object keywordSource = parameters [LookupKeyName];

			if (SplitKeywords && (keywordSource is string))
				keywords = KeywordSplitter.Split ((string)keywordSource);
		 	else 
				keywords = (IEnumerable<string>)keywordSource;

			keywords = Lookups.GetKeylist (keywords);

			thisLookup.Add(new LookupEntry(
				keywords,
				parameters[MetaName].ToString(),
				parameters));

			return true;
		}
	}
}

