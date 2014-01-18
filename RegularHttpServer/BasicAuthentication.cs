using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;
using System.Net;
using System.Text;

namespace RegularHttpServer
{
	public class BasicAuthentication : Service
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

		protected override void Initialize (Settings modSettings)
		{
			byte[] loginBytes = Encoding.ASCII.GetBytes (
				(string)modSettings ["username"] + ":" +
				(string)modSettings ["password"]);

			loginString = Convert.ToBase64String (loginBytes);
			realm = (string)modSettings ["realm"];
			wwwAuthHeader = string.Format ("Basic realm=\"{0}\"", realm);
		}

		protected override bool Process (Interaction parameters)
		{
			string[] authHeader = parameters.IncomingHeaders.GetValues ("Authorization");

			if ((authHeader != null) &&
				(authHeader.Length > 0)) {

				authHeader = authHeader [0].Split (' ');

				if ((authHeader [0] == "Basic") &&
					(authHeader [1] == loginString)) {
					return RunBranch ("http", parameters);
				}
			} 

			parameters.StatusCode = 401;
			parameters.OutgoingHeaders ["WWW-Authenticate"] = wwwAuthHeader;

			return true;
		}
	}
}

