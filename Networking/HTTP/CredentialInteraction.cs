using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Net;

namespace Networking
{
	class CredentialInteraction : BareInteraction
	{
		public ICredentials Credentials {
			get;
			set;
		}

		public CredentialInteraction (ICredentials credentials, IInteraction parameters) : base(parameters)
		{
			this.Credentials = credentials;
		}

		public override IInteraction Clone(IInteraction newParent) {
			throw new UnclonableException ();
		}
	}
}

