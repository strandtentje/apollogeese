using System;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Databases
{
	public class SQLRow : SQLList
	{

		string IdentityRow {
			get;
			set;
		}

		protected override void Initialize (Settings modSettings)
		{
			this.IdentityRow = modSettings.GetString ("idrow", "id");

			modSettings ["params"] = new string[] { IdentityRow };
			modSettings ["where"] = string.Format("{0} = @{0}", IdentityRow);

			base.Initialize (modSettings);
		}

		protected override bool Process (BorrehSoft.ApolloGeese.CoreTypes.IInteraction parameters)
		{
			return base.Process (parameters);
		}
	}
}

