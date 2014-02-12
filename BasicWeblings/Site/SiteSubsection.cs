using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;
using System.Net;
using System.Collections.Generic;
using BorrehSoft.Utensils;
using BorrehSoft.ApolloGeese.Duckling.Http;

namespace BorrehSoft.Extensions.BasicWeblings.Site
{
	public class SiteSubsection : Service
	{
		private string[] branches;
		private Map<object> BranchNames = new Map<object>();

		public override string[] AdvertisedBranches {
			get {
				return branches;
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
			branches = new string[BranchNames.Length];
			BranchNames.GetNames ().CopyTo (branches, 0);
		}

		protected override bool Process (IInteraction uncastParameters)
		{
			IHttpInteraction parameters = (IHttpInteraction)uncastParameters;

			string branchId;

			if (parameters.URL.EndOfSeries)	branchId = "";
			else branchId = parameters.URL.ReadUrlChunk ();

			return RunBranch (branchId, parameters);
		}
	}
}

