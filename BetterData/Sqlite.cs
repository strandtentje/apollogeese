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
				return "Data Source={0};Version=3;";
			}
		}

		public override IDbConnection GetNewConnection ()
		{
			return new SqliteConnection (this.ConnectionString);
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			Settings ["dbfile"] = defaultParameter;
		}

		protected override void Initialize (Settings settings)
		{
			string file = settings.GetString ("dbfile", "db.sqlite");
			settings ["connectionstring"] = string.Format (ConnectionStringTemplate, file);
			base.Initialize (settings);
		}
	}
}

