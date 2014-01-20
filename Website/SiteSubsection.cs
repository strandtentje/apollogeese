using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;
using System.Net;
using System.Collections.Generic;

namespace Website
{
	public class SiteSubsection : Service
	{
		public override string[] AdvertisedBranches {
			get {
				return new string[] { "root", "*" };
			}
		}

		public override string Description {
			get {
				return "The root to a webservice which provides webpage data " +
					"for a request.";
			}
		}

		protected override void Initialize (Settings modSettings)
		{

		}

		protected override bool Process (Interaction parameters)
		{
			// Assigns children to 'Siblings' parameter for underlying child.
			parameters.Luggage ["Siblings"] = ConnectedBranches.Values;

			if (!parameters.InvokeForNextURLChunk(delegate(string chunk) {
				return RunBranch (chunk, parameters);
			}))				
				return RunBranch ("root", parameters);		
		}
	}
}

