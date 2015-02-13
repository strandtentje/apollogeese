using System;
using BorrehSoft.ApolloGeese.Duckling;
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
	public class LookupWriter : Service
	{
		/// <summary>
		/// Gets or sets name at which the keywords will reside in interactions
		/// </summary>
		/// <value>The name of the keyword list</value>
		private string LookupKeyName { get; set; }
		/// <summary>
		/// Gets or sets name of lookup to write to
		/// </summary>
		/// <value>The name of the lookup.</value>
		private string LookupName {	get; set; }
		/// <summary>
		/// Gets or sets the name at which the unique meta-identifier resides in 
		/// an interaction
		/// </summary>
		/// <value>The name of the meta.</value>
		private string MetaName { get; set; }
		/// <summary>
		/// Gets or sets the keyword splitting regex
		/// </summary>
		/// <value>The keyword splitter.</value>
		private Regex KeywordSplitter { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="BorrehSoft.Extensions.BasicWeblings.Lookup.LookupWriter"/>
		/// will split keywords or use an existing string enumerable.
		/// </summary>
		/// <value><c>true</c> if split keywords; otherwise, <c>false</c>.</value>
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

