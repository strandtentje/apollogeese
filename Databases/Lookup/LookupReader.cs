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
	public class LookupReader : LookupAccessor
	{
		private Service iterator;
		private Service capreached = Stub;

		[Instruction("Maximum query size in words.", 6)]
		public int KeyCap { get; set; }

		[Instruction("Maximum result size in iterations.", -1)]
		public int ResultCap { get; set; }

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
			LookupKeyName = modSettings.GetString ("lookupkeyname");
			LookupName = modSettings.GetString ("lookupname");
			KeywordSplitterRegex = modSettings.GetString("keywordsplitregex", @"\W|_");
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

