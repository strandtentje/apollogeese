using System;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.Extensions.BasicWeblings.Data.MySQL
{
	public class MySqlQuerier : Querier
	{
		protected override IQueryConnection CreateConnection (Settings modSettings)
		{	
			Settings pickedSettings = modSettings;
			string credsetName;

			if (modSettings.TryGetString ("credset", out credsetName)) {
				pickedSettings = CredentialsStore.Credentials.GetSubsettings (credsetName);
			}

			if (pickedSettings.Has ("connectionstring")) {
				return new MySqlQueryConnection((string)pickedSettings["connectionstring"]);
			}

			return new MySqlQueryConnection(
					(string)pickedSettings ["host"], (string)pickedSettings ["db"], 			                     
					(string)pickedSettings ["user"], (string)pickedSettings ["pass"],
					(bool)pickedSettings.GetBool("pool", true));
		}
	}
}

