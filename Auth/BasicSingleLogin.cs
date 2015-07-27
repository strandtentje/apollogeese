using System;
using System.Text;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Http;

namespace BorrehSoft.ApolloGeese.Extensions.Auth
{
	public class BasicSingleLogin : TwoBranchedService
	{
		private string Username { get; set; }
		private string Password { get; set; }
		private string Realm { get; set; }

		private string LoginString {
			get {
				return "Basic " + Convert.ToBase64String (Encoding.ASCII.GetBytes (string.Format ("{0}:{1}", Username, Password)));
			}
		}

		private string ResponseHeader {
			get {
				return string.Format ("Basic realm=\"{0}\"", this.Realm);
			}
		}

		public override string Description {
			get {
				return string.Format ("authenticate user '{0}' for realm '{1}'", Username, Realm);
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			this.Username = (string)modSettings.Get ("username");
			this.Password = (string)modSettings.Get ("password");
			this.Realm = (string)modSettings.Get ("realm");
		}

		protected override bool Process (IInteraction parameters)
		{
			IHttpInteraction httpParameters = (IHttpInteraction)parameters.GetClosest (typeof(IHttpInteraction));
			string[] authHeader = httpParameters.RequestHeaders.Backend.GetValues ("Authorization");
			bool successful = true ;

			if ((authHeader != null) && (authHeader.Length > 0) && (authHeader [0] == LoginString)) {
				 successful &= Successful.TryProcess (parameters);
			} else {
				httpParameters.SetStatuscode (401);
				httpParameters.ResponseHeaders.Backend.Add ("WWW-Authenticate", ResponseHeader);
				successful &= Failure.TryProcess (parameters);
			}

			return successful;
		}
	}
}

