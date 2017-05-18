using System;
using System.Text;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Auth
{
	public class BasicSingleLogin : TwoBranchedService
	{
		[Instruction("Username this basiclogon will accept")]
		public string Username { get; set; }

		[Instruction("Password this basiclogon will accept")]
		public string Password { get; set; }

		[Instruction("Realm this logon is valid for")]
		public string Realm { get; set; }

		private string LoginString {
			get {
				return "Basic " + Convert.ToBase64String (
					Encoding.ASCII.GetBytes (
						string.Format (
							"{0}:{1}", 
							Username, 
							Password
						)
					)
				);
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
			string authHeader = httpParameters.RequestHeaders["Authorization"];
			bool successful = true ;

			if ((authHeader != null) && (authHeader == LoginString)) {
				 successful &= Successful.TryProcess (parameters);
			} else {
				httpParameters.SetStatusCode (401);
				httpParameters.ResponseHeaders ["WWW-Authenticate"] = ResponseHeader;
				successful &= Failure.TryProcess (parameters);
			}

			return successful;
		}
	}
}

