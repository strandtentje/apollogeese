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
        public bool IsHttpOnly { get; private set; }
        [Instruction("Set cookie expiration", true)]
		public string Expires { get; set; }
		private bool isPersisitent = false;
		private TimeSpan expiryDelta;
		private DateTime actualExpiry;

		public override void LoadDefaultParameters (string defaultParameter)
		{
			this.Settings ["cookiename"] = defaultParameter;
		}

		protected override void Initialize (Settings modSettings)
		{
			this.CookieName = modSettings.GetString ("cookiename", "SES");
			this.Closing = modSettings.GetBool("sessioncloser", false);
			this.IsSecureSession = modSettings.GetBool ("issecure", true);
            this.IsHttpOnly = modSettings.GetBool("ishttponly", true);

			if (modSettings.Has("expires")) {
				this.isPersisitent = true;
				this.Expires = modSettings.GetString("expires", "");
				this.expiryDelta = TimeSpan.Parse(this.Expires);
			}
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

					do { 
						/* i don't sport the statistical capacity to prove a 1024-bit random number is very very 
						 * very very very very very very very rarely going to occur twice, so we'll check if it 
						 * occurred before. */

						cookieValue = Convert.ToBase64String (GetTrue (128));

						Console.WriteLine (string.Format ("cookie length {0}", cookieValue.Length));
					} while (SessionStates.Has(cookieValue));
								
					if (this.isPersisitent) {
						parameters.SetPersistentCookie(CookieName, cookieValue, DateTime.Now.Add(this.expiryDelta), this.IsHttpOnly, this.IsSecureSession);
					} else {
						parameters.SetCookie(CookieName, cookieValue, this.IsHttpOnly, this.IsSecureSession);
					}

					givenCookie = cookieValue;

					SessionStates [cookieValue] = SessionState.Existing;
				}

				success = WithBranch.TryProcess (new SessionInteraction (uncastParameters, CookieName, givenCookie));
			}

			return success;
		}
	}
}

