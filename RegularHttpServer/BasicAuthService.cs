using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;
using System.Net;
using System.Text;

namespace RegularHttpServer
{
	public class BasicAuthService : Service
	{
		public override string[] AdvertisedBranches {
			get {
				return new string[] { "http" };
			}
		}

		public override string Description {
			get {
				return "Adds basic HTTP authentication";
			}
		}

		string loginString;
		string realm;
		string wwwAuthHeader;

		public override void Initialize (Settings modSettings)
		{
			byte[] loginBytes = Encoding.ASCII.GetBytes (
				(string)modSettings ["username"] + ":" +
				(string)modSettings ["password"]);

			loginString = Convert.ToBase64String (loginBytes);
			realm = (string)modSettings ["realm"];
			wwwAuthHeader = string.Format ("Basic realm=\"{0}\"", realm);
		}

		public override bool Process (HttpListenerContext context, Parameters parameters)
		{
			string[] authHeader = context.Request.Headers.GetValues ("Authorization");

			if ((authHeader != null) &&
				(authHeader.Length > 0)) {

				authHeader = authHeader [0].Split (' ');

				if ((authHeader [0] == "Basic") &&
					(authHeader [1] == loginString)) {
					return RunBranch ("http", context, parameters);
				}
			} 

			context.Response.StatusCode = 401;
			context.Response.AddHeader ("WWW-Authenticate", wwwAuthHeader);
			return true;
		}
	}
}

