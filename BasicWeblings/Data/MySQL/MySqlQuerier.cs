using System;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.Extensions.BasicWeblings.Data.MySQL
{
	public class MySqlQuerier : Querier
	{
		protected override IQueryConnection CreateConnection (Settings modSettings)
		{		
			if (modSettings.Has ("connectionstring")) {
				return new MySqlQueryConnection((string)modSettings["connectionstring"]);
			}

			return new MySqlQueryConnection(
					(string)modSettings ["host"], (string)modSettings ["db"], 			                     
					(string)modSettings ["user"], (string)modSettings ["pass"],
					(bool)modSettings.GetBool("pool", true));
		}
	}
}

