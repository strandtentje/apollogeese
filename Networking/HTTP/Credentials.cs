using System;
using System.Net;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;

namespace Networking
{
	public class Credentials : SingleBranchService
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

		protected override bool Process (IInteraction parameters)
		{
			return WithBranch.TryProcess (new CredentialInteraction (this.CurrentCredentials, parameters));
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

