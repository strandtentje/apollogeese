﻿using System;
using BCryptHasher = BCrypt.Net.BCrypt;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

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
				this.FromVariable, 
				out password) & this.Successful.TryProcess (
					new SimpleInteraction (
						parameters, 
						this.ToVariable,
						BCryptHasher.HashPassword (
							password)));
		}
	}
}

