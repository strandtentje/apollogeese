using System;
using System.Collections.Generic;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections;
using System.Security.Cryptography;

namespace BorrehSoft.ApolloGeese.Auth
{
	/// <summary>
	/// Tags interactions with a (new) session cookie
	/// </summary>
	public class Sessionizer : SingleBranchService
	{
		public override string Description {
			get {
				return string.Format(
					"Session cookie keeper for '{0}'", 
					CookieName);
			}
		}

		public enum SessionState
		{
			Existing, Closed
		}

		/// <summary>
		/// The known sessions cookie strings
		/// </summary>
		public static Map<SessionState> SessionStates = new Map<SessionState>();

		[Instruction("When set to true, this will revoke the session", false)]
		public bool Closing { get; set; }

		[Instruction("Name that is used for this cookie in browser and server context.", "SES")]
		public string CookieName { get; set; }

		[Instruction("Secure cookie header", true)]
		public bool IsSecureSession { get; set; }

		public override void LoadDefaultParameters (string defaultParameter)
		{
			this.Settings ["cookiename"] = defaultParameter;
		}

		protected override void Initialize (Settings modSettings)
		{
			this.CookieName = modSettings.GetString ("cookiename", "SES");
			this.Closing = modSettings.GetBool("sessioncloser", false);
			this.IsSecureSession = modSettings.GetBool ("issecure", true);
		}

		private static RandomNumberGenerator rng = new RNGCryptoServiceProvider ();

		public static byte[] GetTrue (int i)
		{
			byte[] bytes = new byte[i];
			rng.GetBytes (bytes);
			return bytes;
		}

		protected override bool Process (IInteraction uncastParameters)
		{
			bool success = false;
			
			IInteraction candidateParameters;
			IHttpInteraction parameters;

			if (((candidateParameters = uncastParameters.Parent) is IHttpInteraction) || uncastParameters.TryGetClosest (typeof(IHttpInteraction), out candidateParameters)) {
				parameters = (IHttpInteraction)candidateParameters;

				string givenCookie = parameters.GetCookie (CookieName);

				if (givenCookie != null) {
					if (Closing) {
						SessionStates [givenCookie] = SessionState.Closed;
					}

					if (SessionStates [givenCookie] == SessionState.Closed) {
						givenCookie = "";
					}
				}

				if (Closing && (givenCookie != null)) {
					SessionStates [givenCookie] = SessionState.Closed;
				}



				if ((givenCookie ?? "").Length == 0) {   // an empty sescookie
					// we create a cookie

					string cookieValue;

					do { // we have a loop here for a stupidly rare case that probably only occurs
						// when we don't expect it to, so there.
						// The Base64 thing is there to make sure only letters and numbers
						// end up in the cookie.

						cookieValue = Convert.ToBase64String (GetTrue (128));

						Console.WriteLine (string.Format ("cookie length {0}", cookieValue.Length));

					} while (SessionStates.Has(cookieValue));
								
					parameters.SetCookie (CookieName, cookieValue, this.IsSecureSession);

					givenCookie = cookieValue;

					SessionStates [cookieValue] = SessionState.Existing;

					// Yes, I made the Sescookie-creation loop around in case of a duplicate
					// gloBALLY UNIQUE IDENTIFIER now hand me my tinfoil hat.
				}

				success = WithBranch.TryProcess (new SessionInteraction (uncastParameters, CookieName, givenCookie));
			}

			return success;
		}
	}
}

