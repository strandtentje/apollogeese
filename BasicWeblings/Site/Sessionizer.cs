using System;
using System.Collections.Generic;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.ApolloGeese.Duckling.Http;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.Extensions.BasicWeblings.Site
{
	/// <summary>
	/// Tags interactions with a (new) session cookie
	/// </summary>
	public class Sessionizer : Service
	{
		/// <summary>
		/// The known sessions cookie strings
		/// </summary>
		public List<string> knownSessions = new List<string> ();

		private TimeSpan cookieLife = new TimeSpan (1, 0, 0);
		private string cookieName = "SES";

		public override string Description {
			get {
				return "Attaches/Reads out a session cookie";
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			Branches ["http"] = Stub;

			string temporary;

			if (modSettings.TryGetString ("CookieLife", out temporary))
				cookieLife = TimeSpan.Parse (temporary);

			if (modSettings.TryGetString ("CookieName", out temporary))
				cookieName = temporary;
		}

		protected override bool Process (IInteraction uncastParameters)
		{
			IHttpInteraction parameters = (IHttpInteraction)uncastParameters;

			string givenCookie = parameters.RequestHeaders.Cookies [cookieName];

			if ((givenCookie == null) ||		// In case of a null sescookie
				(givenCookie.Length == 0) ||    // an empty sescookie
				(!knownSessions.Contains (givenCookie))) {  // or an unknown session cookie
				// we create a cookie

				string cookieValue;

				do { // we have a loop here for a stupidly rare case that probably only occurs
					 // when we don't expect it to, so there.
					 // The Base64 thing is there to make sure only letters and numbers
					 // end up in the cookie. C#'s got a useful Guid-creation method so we
					 // don't have to think that up.

					cookieValue = Convert.ToBase64String (
						Guid.NewGuid ().ToByteArray ());
				} while (knownSessions.Contains(cookieValue));
								
				parameters.ResponseHeaders.SetCookie (cookieName, cookieValue);

				// Yes, I made the Sescookie-creation loop around in case of a duplicate
				// gloBALLY UNIQUE IDENTIFIER now hand me my tinfoil hat.
			}

			parameters ["Session"] = givenCookie;

			return Branches["http"].TryProcess(parameters);
		}
	}
}

