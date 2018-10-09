using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using System.Security.Cryptography;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

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

		RandomNumberGenerator generator = RandomNumberGenerator.Create();
		int Length = 64;
		string VariableName = "nonce";

		public string UseCharacters { get; private set; }

		protected override void Initialize (Settings settings)
		{
			this.Length = int.Parse(settings.GetString("length"));
			this.VariableName = settings.GetString("variablename");
			this.UseCharacters = settings.GetString(
				"usecharacters", 
				"abcdefghijklmnopqrstuvwxyz" +
				"ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
				"0123456789"
			);
		}

		protected override bool Process (IInteraction parameters)
		{
			byte[] targetNonce = new byte[this.Length];            
			string nonce;
            generator.GetBytes(targetNonce);
			nonce = string.Join("", targetNonce.Select(
				b => this.UseCharacters[b ^ 255]
			));               

			return WithBranch.TryProcess (new SimpleInteraction (parameters, this.VariableName, nonce));
		}
	}
}

