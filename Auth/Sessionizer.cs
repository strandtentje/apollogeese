using System;
using System.Collections.Generic;
using URandom = BorrehSoft.Utensils.Random;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.Auth
{
	/// <summary>
	/// Tags interactions with a (new) session cookie
	/// </summary>
	public class Sessionizer : Service
	{
		public override string Description {
			get {
				return string.Format("Session cookie keeper for '{0}'", CookieName);
			}
		}

		/// <summary>
		/// The known sessions cookie strings
		/// </summary>
		public static List<string> knownSessions = new List<string> ();

		private Service Http;

		[Instruction("When set to true, this will revoke the session", false)]
		public bool Closing { get; set; }

		[Instruction("Name that is used for this cookie in browser and server context.", "SES")]
		public string CookieName { get; set; }

		public override void LoadDefaultParameters (string defaultParameter)
		{
			this.Settings ["cookiename"] = defaultParameter;
		}

		protected override void Initialize (Settings modSettings)
		{
			Branches ["http"] = Stub;

			this.CookieName = modSettings.GetString ("cookiename", "SES");
			this.Closing = modSettings.GetBool("sessioncloser", false);
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "http") 
				Http = e.NewValue;
		}

		protected override bool Process (IInteraction uncastParameters)
		{
			IHttpInteraction parameters = (IHttpInteraction)uncastParameters.GetClosest (typeof(IHttpInteraction));

			string givenCookie = parameters.GetCookie (CookieName);

			if (Closing && (givenCookie != null)) {
				knownSessions.Remove(givenCookie);
			}

			if ((givenCookie ?? "").Length == 0)   // an empty sescookie
			{
				// we create a cookie

				string cookieValue;

				do { // we have a loop here for a stupidly rare case that probably only occurs
					 // when we don't expect it to, so there.
					 // The Base64 thing is there to make sure only letters and numbers
					 // end up in the cookie.

					cookieValue = Convert.ToBase64String (URandom.GetTrue(128));

					Console.WriteLine(string.Format("cookie length {0}", cookieValue.Length));

				} while (knownSessions.Contains(cookieValue));
								
				parameters.SetCookie (CookieName, cookieValue);

				givenCookie = cookieValue;

				knownSessions.Add(givenCookie);

				// Yes, I made the Sescookie-creation loop around in case of a duplicate
				// gloBALLY UNIQUE IDENTIFIER now hand me my tinfoil hat.
			}

			return Http.TryProcess(new SessionInteraction(uncastParameters, CookieName, givenCookie));
		}
	}
}

