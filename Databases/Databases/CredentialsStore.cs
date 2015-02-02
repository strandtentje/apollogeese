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

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			throw new Exception ("Credential storage isn't meant for this :(");
		}

		protected override void Initialize (Settings modSettings)
		{
			string credset = (string)modSettings ["credset"];
			CredentialsLookup [credset] = modSettings;
		}

		protected override bool Process (IInteraction parameters)
		{
			throw new Exception ("Credential storage isn't meant for this either ::((");
		}
	}
}

