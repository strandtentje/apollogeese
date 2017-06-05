using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Text;
using BorrehSoft.Utilities.Collections.Settings;

namespace Auth
{
	public class BasicAuthentication : TwoBranchedService
	{
		[Instruction("Realm this logon is valid for")]
		public string Realm { get; set; }

		private string ResponseHeader {
			get {
				return string.Format ("Basic realm=\"{0}\"", this.Realm);
			}
		}

		public override string Description {
			get {
				return string.Format ("authenticate for realm '{0}'", Realm);
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			Settings["realm"] = defaultParameter;
		}

		protected override void Initialize (Settings modSettings)
		{
			this.Realm = (string)modSettings.Get ("realm");
		}

		protected override bool Process (IInteraction parameters)
		{
			/* return "Basic " + Convert.ToBase64String (
				Encoding.ASCII.GetBytes (
					string.Format (
						"{0}:{1}", 
						Username, 
						Password
					)
				)
			); */

			bool successful = true;

			IHttpInteraction httpParameters = (IHttpInteraction)parameters.GetClosest (typeof(IHttpInteraction));
			string authHeader = httpParameters.RequestHeaders["Authorization"];

			if ((authHeader != null) && authHeader.StartsWith("Basic ")) {
				string encAuthString = authHeader.Substring ("Basic ".Length).Trim ();
				string authString = Encoding.ASCII.GetString (Convert.FromBase64String (encAuthString));
				string[] userPass = authString.Split (':');

				if (userPass.Length == 2) {
					var credentials = new SimpleInteraction (parameters);

					credentials ["username"] = userPass [0];
					credentials ["password"] = userPass [1];

					successful &= Successful.TryProcess (credentials);
				} else {
					successful &= Failure.TryProcess (parameters);
				}
			} else {
				httpParameters.SetStatusCode (401);
				httpParameters.ResponseHeaders ["WWW-Authenticate"] = ResponseHeader;
				successful &= Failure.TryProcess (parameters);
			}

			return successful;
		}
	}
}

