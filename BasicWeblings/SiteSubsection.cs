using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;
using System.Net;
using System.Collections.Generic;
using BorrehSoft.Utensils;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class SiteSubsection : Service
	{
		private Map<string> BranchNames = new Map<string>();

		public override string[] AdvertisedBranches {
			get {
				return BranchNames.GetNames ();
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
			BranchNames = modSettings;
		}

		protected override bool Process (HttpInteraction parameters)
		{
			string originalTitle = parameters.CurrentTitle;
			string branchId;
			bool success;

			if (parameters.URL.EndOfSeries)	branchId = "";
			else branchId = parameters.URL.ReadUrlChunk ();


			parameters.CurrentTitle = BranchNames [branchId];

			success = RunBranch (branchId, parameters);

			parameters.CurrentTitle = originalTitle;

			return success;
		}
	}
}

