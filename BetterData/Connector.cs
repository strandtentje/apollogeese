using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using System.Data;
using MySql.Data.MySqlClient;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections;

namespace BetterData
{
	class Connector : Service
	{
		string Name;

		string Type;

		string ConnectionString;

		static Map<IDbConnection> NamedConnections = new Map<IDbConnection>();

		public override string Description {
			get {
				return string.Format(
					"Definition for connector {0}", this.Name);
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			Settings ["name"] = defaultParameter;
		}

		protected override void Initialize (Settings settings)
		{
			this.Name = settings.GetString ("name", "default");
			this.Type = settings.GetString ("type", "mysql");
			this.ConnectionString = settings.GetString (
				"connectionstring");

			if (Type == "mysql") {
				NamedConnections [this.Name] = new MySqlConnection (
					this.ConnectionString);
			} else {
				throw new MissingConnectorException (this.Type);
			}
		}

		public static IDbConnection Find (string name)
		{
			return NamedConnections [name];
		}
	}


}

