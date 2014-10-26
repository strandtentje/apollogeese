using System;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;

namespace BorrehSoft.Extensions.BasicWeblings.Data.SQLite
{
	public class SQLiteQuerier : Querier
	{
		Dictionary<string, IQueryConnection> openConnections = new Dictionary<string, IQueryConnection>();

		protected override IQueryConnection CreateConnection (Settings modSettings)
		{
			string dbFileName = modSettings.GetString("databasefile", "db.sqlite3");

			if (!openConnections.ContainsKey(dbFileName))
				openConnections.Add(dbFileName, new SQLiteQueryConnection(dbFileName));

			return openConnections[dbFileName];
		}
	}
}
	