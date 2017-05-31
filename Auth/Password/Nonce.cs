using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using System.Security.Cryptography;
using System.Web.Security;
using System.Text.RegularExpressions;

namespace Auth
{
	public class Nonce : SingleBranchService
	{
		public override string Description {
			get {
				return string.Format("Generate {0} character nonce into {1}", Length, VariableName);
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			string[] setupString = defaultParameter.Split('>');

			if (setupString.Length  == 2) {
				Settings["length"] = setupString[0];
				Settings["variablename"] = setupString[1];
			}
		}

		Random rnd = new Random();
		int Length = 64;
		string VariableName = "nonce";

		protected override void Initialize (Settings settings)
		{
			this.Length = int.Parse(settings.GetString("length"));
			this.VariableName = settings.GetString("variablename");
		}

		protected override bool Process (IInteraction parameters)
		{
			var dinkyNonce = Membership.GeneratePassword (this.Length, 0);
			var fixedNonce = Regex.Replace (dinkyNonce, @"[^a-zA-Z0-9]", m => rnd.Next (0, 10).ToString ());
			var nonceInteraction = new SimpleInteraction (parameters, this.VariableName, fixedNonce);
			return WithBranch.TryProcess (nonceInteraction);
		}
	}
}

