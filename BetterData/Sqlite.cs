using System;
using System.Data;
using Mono.Data.Sqlite;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;

namespace BetterData
{
	public class Sqlite : Connector
	{
		public override string ConnectionStringTemplate {
			get {
				return "URI=file:{0};Version=3;";
			}
		}

		public override IDbConnection GetNewConnection ()
		{
			return new SqliteConnection (this.ConnectionString);
		}

		protected override void Initialize (Settings settings)
		{
			string file = settings.GetString ("dbfile", "db.sqlite");
			settings ["connectionstring"] = string.Format (ConnectionStringTemplate, file);
			base.Initialize (settings);
		}
	}
}

