using System;
using System.Collections.Generic;
using URandom = BorrehSoft.Utensils.Random;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.ApolloGeese.Http;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.Auth
{
	/// <summary>
	/// Tags interactions with a (new) session cookie
	/// </summary>
	public class Sessionizer : Service
	{
		/// <summary>
		/// The known sessions cookie strings
		/// </summary>
		public static List<string> knownSessions = new List<string> ();

		private Service Http;
		private TimeSpan cookieLife = new TimeSpan (1, 0, 0);
		private bool closing, checkKnown;
		private string cookieName = "SES";

		public override string Description {
			get {
				return cookieName;
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			Branches ["http"] = Stub;

			string temporary;

			if (modSettings.TryGetString ("cookielife", out temporary))
				cookieLife = TimeSpan.Parse (temporary);

			if (modSettings.TryGetString ("cookiename", out temporary))
				cookieName = temporary;
			else if (modSettings.TryGetString ("default", out temporary))
				cookieName = temporary;

			closing = modSettings.GetBool("sessioncloser", false);
			checkKnown = modSettings.GetBool ("checkknown", false);
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "http") 
				Http = e.NewValue;
		}

		protected override bool Process (IInteraction uncastParameters)
		{
			IHttpInteraction parameters = (IHttpInteraction)uncastParameters.GetClosest (typeof(IHttpInteraction));

			string givenCookie = parameters.RequestHeaders.Cookies.Get (cookieName, null);

			if (closing && (givenCookie != null)) {
				knownSessions.Remove(givenCookie);
			}

			if ((givenCookie == null) ||		// In case of a null sescookie
				(givenCookie.Length == 0) ||    // an empty sescookie
				(checkKnown && !knownSessions.Contains (givenCookie))) {  // or an unknown session cookie
				// we create a cookie

				string cookieValue;

				do { // we have a loop here for a stupidly rare case that probably only occurs
					 // when we don't expect it to, so there.
					 // The Base64 thing is there to make sure only letters and numbers
					 // end up in the cookie.

					cookieValue = Convert.ToBase64String (URandom.GetTrue(128));

					Console.WriteLine(string.Format("cookie length {0}", cookieValue.Length));

				} while (knownSessions.Contains(cookieValue));
								
				parameters.ResponseHeaders.SetCookie (cookieName, cookieValue);

				givenCookie = cookieValue;

				knownSessions.Add(givenCookie);

				// Yes, I made the Sescookie-creation loop around in case of a duplicate
				// gloBALLY UNIQUE IDENTIFIER now hand me my tinfoil hat.
			}

			return Http.TryProcess(new SessionInteraction(uncastParameters, cookieName, givenCookie));
		}
	}
}

