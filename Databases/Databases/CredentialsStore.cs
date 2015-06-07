using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Databases
{
	/// <summary>
	/// Static logon credentials storage so other services may easily access them using a single
	/// token.
	/// </summary>
	public class CredentialsStore : Service
	{
		public static Map<Settings> CredentialsLookup = new Map<Settings>();

		public override string Description {
			get {
				return "Credential storage, leave unattached.";
			}
		}

		void Illegal() {
			throw new Exception (string.Format("{0} isn't meant for this :(", this.GetType().Name));
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			Illegal ();
		}

		protected override void Initialize (Settings modSettings)
		{
			string credset = (string)modSettings ["credset"];
			CredentialsLookup [credset] = modSettings;
		}

		protected override bool Process (IInteraction parameters)
		{
			Illegal ();
			return true;
		}
	}
}

