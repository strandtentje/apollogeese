using System;
using System.Data;

namespace BorrehSoft.Extensions.BasicWeblings.Data.MySQL
{
	/// <summary>
	/// Command for MySQL connection
	/// </summary>
	public class MySqlQueryCommand : IQueryCommand
	{
		/// <summary>
		/// Underlying IDbCommand
		/// </summary>
		/// <value>The command.</value>
		public IDbCommand Command { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Extensions.BasicWeblings.Data.MySQL.MySqlQueryCommand"/> class,
		/// using a connection and query text.
		/// </summary>
		/// <param name="queryConnection">Query connection.</param>
		/// <param name="defaultQueryText">Default query text.</param>
		public MySqlQueryCommand (MySqlQueryConnection queryConnection, string defaultQueryText)
		{
			Command = queryConnection.Connection.CreateCommand ();
			Command.CommandText = defaultQueryText;
			Command.Prepare ();
		}

		/// <summary>
		/// Sets a parameter in the parameterized query.
		/// </summary>
		/// <returns>The command parameter.</returns>
		/// <param name="command">Command.</param>
		/// <param name="name">Name.</param>
		/// <param name="value">Value.</param>
		public IDbDataParameter SetParameter (string name, object value)
		{
			IDbDataParameter parameter = Command.CreateParameter();
			parameter.ParameterName = name;
			parameter.Value = value;
			Command.Parameters.Add(parameter);

			return parameter;
		}

		/// <summary>
		/// Run this instance.
		/// </summary>
		public IDataReader Run()
		{
			return Command.ExecuteReader();
		}
	}
}

