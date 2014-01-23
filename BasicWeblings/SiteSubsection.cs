using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;
using System.Net;
using System.Collections.Generic;
using BorrehSoft.Utensils;

namespace Website
{
	public class SiteSubsection : Service
	{
		private Map<string> Branches = new Map<string>();

		public override string[] AdvertisedBranches {
			get {
				return Branches.GetNames ();
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
			parameters.ExtendSiblings (ConnectedBranches.Values);

			bool success;

			if (!parameters.InvokeForNextURLChunk(delegate(string chunk) {
				success = RunBranch (chunk, parameters);
			}))
				success = RunBranch ("root", parameters);

			return success;
		}
	}
}

