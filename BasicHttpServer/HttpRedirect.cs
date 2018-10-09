using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Log;

namespace BasicHttpServer 
{
	class HttpRedirect : TwoBranchedService 
	{
		string URL;
        string URLVariable;
              
        public override string Description {
            get {
                return "HTTP Redirect";
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
		}

		protected override bool Process(IInteraction parameters)
		{
			if (URLVariable.Length > 0) {
                string contextUrl;
                if (parameters.TryGetFallbackString (URLVariable, out contextUrl)) {
					Closest<IHttpInteraction>.From(parameters).SetRedirect(contextUrl);
					return Successful?.TryProcess(parameters) ?? true;
                } else {
					Secretary.Report(5, "failed to redirect for variable", URLVariable);
                    return Failure?.TryProcess (parameters) ?? true;
                }
            } else if (URL.Length > 0) {
				Closest<IHttpInteraction>.From(parameters).SetRedirect(URL);
				return Successful?.TryProcess(parameters) ?? true;
			} else {
				return Failure?.TryProcess(parameters) ?? true;
			}
		}
	}
}