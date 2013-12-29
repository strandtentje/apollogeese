using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;
using System.Net;

namespace Website
{
	public class WebService : Service
	{
		public override string[] AdvertisedBranches {
			get {
				return new string[] { "section_#" };
			}
		}

		public override string Description {
			get {
				return "The root to a webservice which provides webpage data " +
					"for a request.";
			}
		}

		string serviceRootUrl;

		public override void Initialize (Settings modSettings)
		{
			serviceRootUrl = (string)modSettings ["rooturl"];
		}

		public override bool Process (HttpListenerContext context, Parameters parameters)
		{
			if (!context.Request.RawUrl.StartsWith(serviceRootUrl)
		}
	}
}

