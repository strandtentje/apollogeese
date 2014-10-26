using System;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.Extensions.BasicWeblings.Data.MySQL
{
	public class MySqlQuerier : Querier
	{
		protected override IQueryConnection CreateConnection (Settings modSettings)
		{		
			return new MySqlQueryConnection(
					(string)modSettings ["host"], (string)modSettings ["db"], 			                     
					(string)modSettings ["user"], (string)modSettings ["pass"],
					(bool)modSettings.GetBool("pool", true));
		}
	}
}

