using System;
using System.Data;
using Mono.Data.Sqlite;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace BetterData
{
	public class Sqlite : Connector
	{
		public override string ConnectionStringTemplate {
			get {
				return "Data Source=file:{0}.db";
			}
		}

		public override IDbConnection GetNewConnection ()
		{
			return new SqliteConnection (this.ConnectionString);
		}

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);
		}
	}
}

