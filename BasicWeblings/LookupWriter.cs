using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils;
using System.Collections.Generic;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class LookupWriter : Service
	{
		private string LookupKeyName { get; set; }
		private string LookupName {	get; set; }
		private MultiDict<string, IInteraction> thisLookup;
		static private Dictionary<string, MultiDict<string, IInteraction>> lookupLookup = new Dictionary<string, MultiDict<string, IInteraction>>();

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

			if (lookupLookup.ContainsKey (LookupName))
				thisLookup = lookupLookup [LookupName];
			else {
				thisLookup = new MultiDict<string, IInteraction>();
				lookupLookup.Add(LookupName, thisLookup);
			}
		}

		protected override bool Process (IInteraction parameters)
		{

			return false;
		}
	}
}

