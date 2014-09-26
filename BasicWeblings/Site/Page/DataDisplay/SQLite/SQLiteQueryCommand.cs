using System;
using Mono.Data.Sqlite;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page.DataDisplay
{
	class SQLiteQueryCommand : IQueryCommand
	{
		SqliteCommand sqLiteCommand;

		public SQLiteQueryCommand (string queryText, SqliteConnection connection)
		{
			sqLiteCommand = new SqliteCommand(queryText, connection);
		}

		public IDbCommand Command {
			get { 
				return sqLiteCommand;
			}
		}

		public IDbDataParameter SetParameter (string name, object value)
		{
			IDbDataParameter parameter = Command.CreateParameter();
			parameter.ParameterName = name;
			parameter.Value = value;
			Command.Parameters.Add(parameter);
			return parameter;
		}

		public IDataReader Run ()
		{
			return Command.ExecuteReader();
		}
	}

}

