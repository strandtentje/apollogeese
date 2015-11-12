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

		static Map<Connector> NamedConnectors = new Map<Connector>();

		public override string Description {
			get {
				return string.Format(
					"Definition for connector {0}", this.Name);
			}
		}

		const string simpleMySqlCon = 
			"Server=localhost; " +
			"Database={0}; " +
			"User ID={0}; " +
			"Password={0}; " +
			"Pooling=true; " +
			"Allow User Variables=True";

		public override void LoadDefaultParameters (string defaultParameter)
		{
			if (defaultParameter.ToLower ().Contains("database=")) {
				Settings ["connectionstring"] = defaultParameter;
			} else {
				Settings ["name"] = defaultParameter;
				Settings ["connectionstring"] = string.Format (simpleMySqlCon, defaultParameter);
			}
		}

		protected override void Initialize (Settings settings)
		{
			this.Name = settings.GetString ("name", "default");
			this.ConnectionString = settings.GetString ("connectionstring");

			NamedConnectors [this.Name] = this;
		}

		public virtual IDbConnection GetNewConnection() {	
			IDbConnection connection = new MySqlConnection (
				this.ConnectionString);

			return connection;
		}

		public static IDbConnection Find (string name)
		{
			return NamedConnectors [name].GetNewConnection ();
		}
	}


}

