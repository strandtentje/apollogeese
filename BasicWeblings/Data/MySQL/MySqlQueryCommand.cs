using System;
using System.Data;

namespace BorrehSoft.Extensions.BasicWeblings.Data.MySQL
{
	public class MySqlQueryCommand : IQueryCommand
	{
		public IDbCommand Command { get; private set; }

		public MySqlQueryCommand (MySqlQueryConnection queryConnection, string defaultQueryText)
		{
			Command = queryConnection.Connection.CreateCommand ();
			Command.CommandText = defaultQueryText;
			Command.Prepare ();
		}

		public IDbDataParameter SetParameter (string name, object value)
		{
			IDbDataParameter parameter = Command.CreateParameter();
			parameter.ParameterName = name;
			parameter.Value = value;
			Command.Parameters.Add(parameter);

			return parameter;
		}

		public IDataReader Run()
		{
			return Command.ExecuteReader();
		}
	}
}

