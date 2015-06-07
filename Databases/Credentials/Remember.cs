using System;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Databases
{
	public class Remember : CredentialsStore
	{
		protected override void Initialize (Settings modSettings)
		{
			if ((!modSettings.Has("credset")) && modSettings.Has ("default")) {
				modSettings ["credset"] = modSettings ["default"];
			}

			base.Initialize (modSettings);
		}
	}
}

	