using System;
using Mono.Data.Sqlite;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Data.Common;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Databases.SQLite
{
	public class SQLiteQueryConnection : IQueryConnection
	{
		string connectionString, queryText;
		List<string> defaultParams;
		SqliteConnection connection;

		public SQLiteQueryConnection (string file)
		{
			connectionString = string.Format("URI=file:{0}", file);
			connection = new SqliteConnection(ConnectionString);
			connection.Open();
		}

		public string ConnectionString { get { return connectionString; } }
		
		/// <summary>
		/// Gets the connection.
		/// </summary>
		/// <value>
		/// The connection.
		/// </value>
		public IDbConnection Connection { get { return connection; } }
				
		/// <summary>
		/// Gets the query text.
		/// </summary>
		/// <value>
		/// The query text.
		/// </value>
		public string DefaultQueryText { get { return queryText; } }

		/// <summary>
		/// Gets the ordered parameters.
		/// </summary>
		/// <value>
		/// The ordered parameters.
		/// </value>
		public List<string> DefaultOrderedParameters { get { return defaultParams; } }

		public void SetDefaultCommandQuery(string QueryTextFile, List<string> queryParameters = null) 
		{
			this.queryText = File.ReadAllText(QueryTextFile);
			this.defaultParams = queryParameters ?? new List<string>();
		}
				
		public IQueryCommand GetDefaultCommand() 
		{
			return new SQLiteQueryCommand(this.DefaultQueryText, this.connection);
		}
	}
}

