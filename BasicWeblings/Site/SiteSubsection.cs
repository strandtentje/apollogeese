using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Duckling.Http;

namespace BorrehSoft.Extensions.BasicWeblings.Site
{
	public class SiteSubsection : Service
	{
		public override string Description {
			get {
				return "The root to a webservice which provides webpage data " +
					"for a request.";
			}
		}

		protected override void Initialize (Settings modSettings)
		{

		}

		protected override bool Process (IInteraction uncastParameters)
		{
			IHttpInteraction parameters = (IHttpInteraction)uncastParameters;

			string branchId;

			if (parameters.URL.EndOfSeries)	branchId = "main";
			else branchId = parameters.URL.ReadUrlChunk ();

			return Branches [branchId].TryProcess (uncastParameters); // RunBranch (branchId, parameters);
		}
	}
}

