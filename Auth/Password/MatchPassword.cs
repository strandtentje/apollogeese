using System;
using BCryptHasher = BCrypt.Net.BCrypt;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace BorrehSoft.ApolloGeese.Auth
{
	public class MatchPassword : PasswordService
	{
		public override string Description {
			get {
				return string.Format(
					"Password matcher using {0}",
					this.Algorithm);
			}
		}


		protected override bool Process (IInteraction parameters)
		{
			string password = "";
			string passwordhash = "";

			if (parameters.TryGetFallbackString (
				this.PlainVariable, out password)) {
				if (parameters.TryGetFallbackString (
					this.HashVariable, out passwordhash)) {
					if (BCryptHasher.Verify (password, passwordhash)) {
						return Successful.TryProcess (parameters);
					}
				}
			}

			return Failure.TryProcess (parameters);
		}		
	}
}

