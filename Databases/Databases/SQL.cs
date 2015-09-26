using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Extensions.Data.Databases.MySQL;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Databases
{
	public class SQL : Service
	{
		DataQuerier pickedQuerier = null;

		public override string Description {
			get {
				if (pickedQuerier == null) {
					return "Generic SQL querier (undecisive)";
				} else {
					return string.Format ("{0} (Dynamically elected)", pickedQuerier.Description);
				}
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			if (modSettings.Has ("credset")) {
				string credentialsName = (string)modSettings ["credset"];
				Settings credentials;
				credentials = CredentialsStore.InformationLookup [credentialsName];

				string databaseType = (string)credentials ["dbtype"];

				if (databaseType == "mysql")
					pickedQuerier = new DataQuerier (MySQL.MySql.CreateConnection (modSettings));
				else 
					throw new QueryException (string.Format (
						"Datbase type {0} has not (yet) been implemented", 
						databaseType));
			} else {
				throw new QueryException (
					"'credset' is a neccesary for the generic SQL Service to function");
			}

			pickedQuerier.SetBranches (this.Branches);
			pickedQuerier.SetSettings (modSettings);
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}

		protected override bool Process (IInteraction parameters)
		{
			return pickedQuerier.ProcessDiscretely (parameters);
		}
	}
}
