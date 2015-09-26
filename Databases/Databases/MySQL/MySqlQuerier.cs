using System;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Databases.MySQL
{
	/// <summary>
	/// A querier for MySQL-like databases. Works for PostgreSQL too, last time I checked
	/// </summary>
	public class MySql
	{
		/// <summary>
		/// Creates the connection.
		/// </summary>
		/// <returns>The connection.</returns>
		/// <param name="modSettings">Mod settings.</param>
		public static IQueryConnection CreateConnection (Settings modSettings)
		{	
			Settings pickedSettings = modSettings;
			string credsetName;

			if (modSettings.TryGetString ("credset", out credsetName)) {
				pickedSettings = CredentialsStore.InformationLookup [credsetName];
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

