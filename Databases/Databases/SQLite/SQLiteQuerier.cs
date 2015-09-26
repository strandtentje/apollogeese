using System;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Databases.SQLite
{
	public class SQLite : Querier
	{
		static Dictionary<string, IQueryConnection> openConnections = new Dictionary<string, IQueryConnection>();

		public static IQueryConnection CreateConnection (Settings modSettings)
		{
			string dbFileName = modSettings.GetString("databasefile", "db.sqlite3");

			if (!openConnections.ContainsKey(dbFileName))
				openConnections.Add(dbFileName, new SQLiteQueryConnection(dbFileName));

			return openConnections[dbFileName];
		}
	}
}
	