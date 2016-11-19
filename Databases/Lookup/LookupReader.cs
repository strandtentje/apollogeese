using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities;
using System.Collections.Generic;
using BorrehSoft.Utilities.Collections.Maps.Search;
using BorrehSoft.Utilities.Collections;
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
		private Service count = null;

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
			if (e.Name == "count")
				this.count = e.NewValue;
		}

		protected override void Initialize (Settings modSettings)
		{
			base.Initialize (modSettings);
			KeyCap = modSettings.GetInt("keycap", 6);
			ResultCap = modSettings.GetInt ("resultcap", -1);
		}

		protected override bool Process (IInteraction parameters)
		{
			string queryText; int resultCount = 0;
			bool success = true;

			if (parameters.TryGetFallbackString (this.LookupKeyName, out queryText)) {
				IEnumerable<string> keylist = Lookups.GetKeylist (Splitter.Split(queryText), KeyCap);
				CleverSet<LookupEntry> results =  Lookups.Get(LookupName).Find (keylist);

				foreach (LookupEntry result in results.Values) {
					if ((ResultCap != -1) && (resultCount++ >= ResultCap)) {
						success &= this.capreached.TryProcess (parameters);
						break;
					} else {
						success &= this.iterator.TryProcess (result.Parameters.Clone (parameters));
					}
				} 
			}

			if (count != null) {
				// jezus wat staat hier
				success &= this.count.TryProcess (
					new SimpleInteraction (
						parameters, "count", 
						Lookups.Get (
							LookupName).AllItems.Count));
			}

			return success;
		}
	}
}

