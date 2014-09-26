using System;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page.DataDisplay
{
	public class SQLiteQuerier : Querier
	{
		protected override IQueryConnection CreateConnection (Settings modSettings)
		{
			return new SQLiteQueryConnection(modSettings.GetString("databasefile", "db.sqlite3"));
		}
	}
}

