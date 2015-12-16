using System;
using System.Net;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace Networking
{
	public class Credentials : Service
	{
		public override string Description {
			get {
				return "Credentials";
			}
		}

		public string Username {
			get;
			set;
		}

		public string Password {
			get;
			set;
		}

		Service Continue {
			get;
			set;
		}

		NetworkCredential CurrentCredentials {
			get;
			set;
		}

		protected override void Initialize (Settings settings)
		{
			this.Username = settings.GetString ("username");
			this.Password = settings.GetString ("password");
			this.CurrentCredentials = new NetworkCredential (this.Username, this.Password);
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "continue") {
				this.Continue = e.NewValue;
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			return this.Continue.TryProcess (new CredentialInteraction (this.CurrentCredentials, parameters));
		}		

		public static ICredentials Recover(IInteraction parameters) {			
			IInteraction credentialCandidate;
			if (parameters.TryGetClosest (
				typeof(CredentialInteraction), 
				out credentialCandidate)
			    ) {
				return ((CredentialInteraction)credentialCandidate).Credentials;
			} else {
				throw new Exception ("Failed to acquire credentials from context");
			}
		}
	}
}

