using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps.Search;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Lookup
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
		[Instruction("Lookup name to mutate")]
		public string LookupName {	get; set; }

		/// <summary>
		/// Gets or sets the name at which the meta-id resides at the parameters
		/// </summary>
		/// <value>The name of the meta.</value>
		[Instruction("Identity attribute of lookup entry.")]
		public string MetaName { get; set; }

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
			this.LookupName = modSettings.GetString ("lookupname");
			this.MetaName = modSettings.GetString ("metaname");

			thisLookup = Lookups.Get(LookupName);
		}

		protected override bool Process (IInteraction parameters)
		{
			thisLookup.RemoveByMeta(parameters[this.MetaName] as String);

			return true;
		}
	}
}

