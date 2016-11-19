using System;
using BCryptHasher = BCrypt.Net.BCrypt;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Auth
{
	public class HashPassword : PasswordService
	{
		public override string Description {
			get {
				return string.Format(
					"Password hasher using {0}",
					this.Algorithm);
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			string password = "";

			return parameters.TryGetFallbackString (
				this.PlainVariable, 
				out password) & this.Successful.TryProcess (
					new SimpleInteraction (
						parameters, 
						this.HashVariable,
						BCryptHasher.HashPassword (
							password)));
		}
	}
}

