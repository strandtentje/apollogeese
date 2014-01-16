using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;

namespace Website
{
	public class Sessionizer : Service
	{
		public override string[] AdvertisedBranches {
			get {
				return new string[] { "http" };
			}
		}

		public override string Description {
			get {
				return "Attaches/Reads out a session cookie";
			}
		}

		protected override void Initialize (Settings modSettings)
		{

		}

		protected override bool Process (Interaction parameters)
		{
			string givenCookie = parameters.BaseRequest.Cookies ["SES"].Value;

			if ((givenCookie != null) && (givenCookie.Length > 0)) {

			}

			string ipAddr = parameters.BaseRequest.RemoteEndPoint.Address.ToString ();
			string usrClient = parameters.BaseRequest.UserAgent.ToString ();

		}
	}
}

