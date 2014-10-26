using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections.Maps.Search;
using BorrehSoft.Utensils.Collections;

namespace BorrehSoft.Extensions.BasicWeblings.Lookup
{
	public class LookupReader : Service
	{
		private Service iterator;
		private string LookupKeyName { get; set; }
		private string LookupName {	get; set; }
		private int KeyCap { get; set; }
		private SearchMap<LookupEntry> thisLookup;

		public override string Description {
			get {
				return "Allows for looking up earlier written data to named lookup " + this.LookupName;
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "iterator") {
				this.iterator = e.NewValue;
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			LookupKeyName = modSettings ["lookupkeyname"] as String;
			LookupName = modSettings ["lookupname"] as String;
			KeyCap = modSettings.GetInt("keycap", 6);
			thisLookup = Lookups.Get(LookupName);
		}

		protected override bool Process (IInteraction parameters)
		{
			/*
			IEnumerable<string> keylist = Lookups.GetKeylist (parameters [this.LookupKeyName], KeyCap);
			CleverSet<LookupEntry> results = thisLookup.Find (keylist);

			foreach (LookupEntry result in results) {
				this.iterator.TryProcess(result.Parameters.Clone(parameters));
			} */

			return false;
		}
	}
}

