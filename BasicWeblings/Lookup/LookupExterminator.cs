using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Extensions.BasicWeblings.Lookup;
using BorrehSoft.Utensils.Collections.Maps.Search;

namespace BorrehSoft.Extensions.BasicWeblings
{
	/// <summary>
	/// Marks lookup-entries as non-existant.
	/// </summary>
	public class LookupExterminator : Service
	{
		/// <summary>
		/// Gets or sets the name of the lookup this exterminator mutates.
		/// </summary>
		/// <value>The name of the lookup.</value>
		private string LookupName {	get; set; }

		/// <summary>
		/// Gets or sets the name at which the meta-id resides at the parameters
		/// </summary>
		/// <value>The name of the meta.</value>
		private string MetaName { get; set; }

		/// <summary>
		/// The actual lookup this exterminator will mutate.
		/// </summary>
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

