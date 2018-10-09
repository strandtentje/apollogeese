using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;

namespace BasicHttpServer
{
	public class HttpOriginRestriction : TwoBranchedService
	{
		string URL;
		string URLVariable;
		bool EnableXFO;
		bool DisableCSP;

		public override string Description {
			get {
				return "HTTP Origin Restriction";
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			Settings ["url"] = defaultParameter;
		}

		protected override void Initialize (Settings settings)
		{
			this.URL = settings.GetString ("url", "");
			this.URLVariable = settings.GetString ("url_override", "");
			this.EnableXFO = settings.GetBool("enablexframeoptions", false);
			this.DisableCSP = settings.GetBool("disablecsp", false);
		}

		protected override bool Process (IInteraction parameters)
		{
			string xFrameOptions = "";
			string cspOptions = "";
			if (URLVariable.Length > 0) {
				string contextUrl;
				if (parameters.TryGetFallbackString (URLVariable, out contextUrl)) {
					xFrameOptions = string.Format ("ALLOW-FROM {0}", contextUrl);
					cspOptions = string.Format ("frame-ancestors {0}", contextUrl);
				} else {
					return Failure.TryProcess (parameters);
				}
			} else if (URL.Length > 0) {
				xFrameOptions = string.Format ("ALLOW-FROM {0}", URL);
				cspOptions = string.Format ("frame-ancestors {0}", URL);
			} else {
				xFrameOptions = "DENY";
				cspOptions = "frame-ancestors 'none'";
			}
            
			var headers = Closest<IHttpInteraction>.From (parameters).ResponseHeaders;
			if (this.EnableXFO == true) {
			    headers ["X-Frame-Options"] = xFrameOptions;
			}
			if (this.DisableCSP == false) {
			    headers ["Content-Security-Policy"] = cspOptions;
			}

			return Successful.TryProcess (parameters);
		}
	}
}

