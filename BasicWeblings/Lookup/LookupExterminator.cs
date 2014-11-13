using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Extensions.BasicWeblings.Lookup;
using BorrehSoft.Utensils.Collections.Maps.Search;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class LookupExterminator : Service
	{
		private string LookupName {	get; set; }
		private string MetaName { get; set; }
		private SearchMap<LookupEntry> thisLookup;

		public override string Description {
			get {
				return this.thisLookup + " by " + this.MetaName;
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}

		protected override void Initialize (Settings modSettings)
		{			
			LookupName = modSettings ["lookupname"] as String;
			MetaName = modSettings ["metaname"] as String;

			thisLookup = Lookups.Get(LookupName);
		}

		protected override bool Process (IInteraction parameters)
		{
			thisLookup.RemoveByMeta(parameters[this.MetaName] as String);

			return true;
		}
	}
}

