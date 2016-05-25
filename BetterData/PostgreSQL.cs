using System;
using System.Data;
using Npgsql;

namespace BetterData
{
	public class PostgreSQL : Connector
	{
		public override string ConnectionStringTemplate {
			get {
				return "Server=127.0.0.1;" +
				"Port=5432;" +
				"Database={0};" +
				"User Id={0};" +
				"Password={0};";
			}
		}

		public override char ParameterSeparator {
			get {
				return ':';
			}
		}

		public override IDbConnection GetNewConnection ()
		{
			return new NpgsqlConnection (this.ConnectionString);
		}
	}
}

