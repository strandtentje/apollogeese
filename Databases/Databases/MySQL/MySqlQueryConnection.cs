using System;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Databases.MySQL
{
	/// <summary>
	/// My sql query connection.
	/// </summary>
	public class MySqlQueryConnection : IQueryConnection
	{
		/// <summary>
		/// The connection string template.
		/// </summary>
		public const string ConnectionStringTemplate = 
			"Server={0}; Database={1}; User ID={2}; Password={3}; Pooling={4}; Allow User Variables=True;";

		/// <summary>
		/// Gets the connection string.
		/// </summary>
		/// <value>
		/// The connection string.
		/// </value>
		public string ConnectionString { get; private set; }
				
		/// <summary>
		/// Gets the connection.
		/// </summary>
		/// <value>
		/// The connection.
		/// </value>
		public IDbConnection Connection { get; private set; }

		/// <summary>
		/// Gets the query text.
		/// </summary>
		/// <value>
		/// The query text.
		/// </value>
		public string DefaultQueryText { get; private set; }

		/// <summary>
		/// Gets the ordered parameters.
		/// </summary>
		/// <value>
		/// The ordered parameters.
		/// </value>
		public List<string> DefaultOrderedParameters { get; private set; }

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="BorrehSoft.Extensions.BasicWeblings.Data.MySQL.MySqlQueryConnection"/> class, using a precomposed connection string.
		/// </summary>
		/// <param name="ConnectionString">Connection string.</param>
		public MySqlQueryConnection (string ConnectionString)
		{
			this.ConnectionString = ConnectionString;
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="BorrehSoft.Extensions.BasicWeblings.Data.MySQL.MySqlQueryConnection"/> class.
		/// </summary>
		/// <param name="databasehost">Database Hostname</param>
		/// <param name="databasename">Name of database</param>
		/// <param name="user">Logon username</param>
		/// <param name="password">Logon password</param>
		/// <param name="usepool">If set to <c>true</c>, uses connection pool.</param>
		public MySqlQueryConnection (string databasehost, string databasename, string user, string password, bool usepool)
		{
			this.ConnectionString = string.Format (ConnectionStringTemplate, databasehost, databasename, user, password, usepool);
		}

		/// <summary>
		/// Sets the default query for this connection
		/// </summary>
		/// <param name="QueryText">Query text.</param>
		/// <param name="queryParameters">Query parameters.</param>
		public void SetDefaultCommandQuery(string QueryText, List<object> queryParameters = null)
		{			
			this.DefaultQueryText = QueryText;

			this.DefaultOrderedParameters = new List<string> ();

			if (queryParameters != null) 
				foreach(object p in queryParameters) 
					this.DefaultOrderedParameters.Add((string)p);
		}

		/// <summary>
		/// Gets the default command for this connection
		/// </summary>
		/// <returns>The default command.</returns>
		public IQueryCommand GetDefaultCommand()
		{
			if ((Connection == null) || (Connection.State == ConnectionState.Broken) || (Connection.State == ConnectionState.Closed)) {				
				Connection = new MySqlConnection(ConnectionString);
				Connection.Open ();
			}

			return new MySqlQueryCommand(this, DefaultQueryText);
		}
	}
}

