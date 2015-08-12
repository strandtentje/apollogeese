using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections.Maps.Search;
using System.Text.RegularExpressions;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Lookup
{
	/// <summary>
	/// Lookup writer.
	/// </summary>
	public class LookupWriter : LookupAccessor
	{

		[Instruction("Context variable that is indicative of lookup entry identity; this is used to tell lookup entries apart.")]
		private string MetaName { get; set; }

		[Instruction("Will assume keywords are presented in context as string, and still need to be split.", true)]
		private bool SplitKeywords { get; set; }

		/// <summary>
		/// The lookup that is mutated by this writer.
		/// </summary>
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
			LookupKeyName = modSettings.GetString ("lookupkeyname");
			LookupName = modSettings.GetString ("lookupname");
			MetaName = modSettings.GetString ("metaname");
			KeywordSplitterRegex = modSettings.GetString("keywordsplitregex", @"\W|_");
			SplitKeywords = modSettings.GetBool ("splitkeywords", true);

			Lookups.DropLookup (LookupName);
			thisLookup = Lookups.Get(LookupName);
		}

		protected override bool Process (IInteraction parameters)
		{
			IEnumerable<string> keywords;
			object keywordSource = parameters [LookupKeyName];

			if (SplitKeywords && (keywordSource is string)) {
				keywords = KeywordSplitter.Split ((string)keywordSource);
			} else if (keywordSource is IEnumerable<object>) {
				IEnumerable<object> keywordSourceEnumerable = (IEnumerable<object>)keywordSource;
				List<string> keywordStrings = new List<string> ();

				foreach (object keywordObject in keywordSourceEnumerable)
					keywordStrings.Add ((string)keywordObject);

				keywords = keywordStrings;
			} else if (keywordSource is IEnumerable<string>) {
				keywords = (IEnumerable<string>)keywordSource;
			} else {
				keywords = new List<string> ();
			}				

			keywords = Lookups.GetKeylist (keywords);

			thisLookup.Add(new LookupEntry(
				keywords,
				parameters[MetaName].ToString(),
				parameters));

			return true;
		}
	}
}

