using System;
using System.Data;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page.DataDisplay
{
	public class QueryCommand
	{
		public IDbCommand Command { get; private set; }

		public QueryCommand (QueryConnection queryConnection, string defaultQueryText)
		{
			Command = queryConnection.Connection.CreateCommand ();
			Command.CommandText = defaultQueryText;
			Command.Prepare ();
		}
		
		/// <summary>
		/// Sets a command parameter.
		/// </summary>
		/// <returns>
		/// The command parameter.
		/// </returns>
		/// <param name='command'>
		/// Command.
		/// </param>
		/// <param name='name'>
		/// Name.
		/// </param>
		/// <param name='value'>
		/// Value.
		/// </param>
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

