using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections.Maps.Search;

namespace BorrehSoft.Extensions.BasicWeblings.Lookup
{
	public class LookupWriter : Service
	{
		private string LookupKeyName { get; set; }
		private string LookupName {	get; set; }
		private string MetaName { get; set; }
		private SearchMap<LookupEntry> thisLookup;

		public override string Description {
			get {
				return "Lookup " + this.LookupName;
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

			thisLookup = Lookups.Get(LookupName);
		}

		protected override bool Process (IInteraction parameters)
		{
			thisLookup.Add(new LookupEntry(
				Lookups.GetKeylist(
				parameters[LookupKeyName] as IEnumerable<string>),
				parameters[MetaName] as String,
				parameters));

			return true;
		}
	}
}

