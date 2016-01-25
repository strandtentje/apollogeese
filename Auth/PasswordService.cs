using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Auth
{
	public abstract class PasswordService : TwoBranchedService
	{
		protected string PlainVariable, HashVariable, Algorithm;

		public override void LoadDefaultParameters (string defaultParameter)
		{
			string[] split = defaultParameter.Split ('>');
			if (split.Length > 0) {
				Settings ["plain"] = split [0];
				if (split.Length > 1) {
					Settings ["hash"] = split [1];
				}
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			this.PlainVariable = modSettings.GetString ("plain", "password");
			this.HashVariable = modSettings.GetString ("hash", "passwordhash");
			this.Algorithm = modSettings.GetString ("algorithm", "bcrypt");

			if (this.Algorithm != "bcrypt") {
				throw new Exception ("bcrypt is the only supported hashing " +
					"algorithm at this time");
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}
	}
}

