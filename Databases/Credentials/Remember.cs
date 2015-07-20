using System;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Databases
{
	public class Remember : CredentialsStore
	{
		public override void LoadDefaultParameters (string defaultParameter)
		{
			this.Settings ["credset"] = defaultParameter;
		}
	}
}

	