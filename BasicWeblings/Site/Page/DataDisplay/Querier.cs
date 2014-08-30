using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Extensions.BasicWeblings.Server;
using L = BorrehSoft.Utensils.Log.Secretary;
using BorrehSoft.ApolloGeese.Duckling.Http;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page.DataDisplay
{
	/// <summary>
	/// Querier; queries.
	/// </summary>
	public class Querier : Service
	{
		private Service none, single, iterator, successful;

		public override string Description {
			get {
				return "Queries and iterates with attached service.";
			}
		}

		/// <summary>
		/// The connection string template.
		/// </summary>
		public const string ConnectionStringTemplate = 
			"Server={0}; Database={1}; User ID={2}; Password={3}; Pooling={4};";

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
		public string QueryText { get; private set; }

		public List<string> OrderedParameters { get; private set; }

		protected override void Initialize (Settings modSettings)
		{
			Branches ["none"] = Stub;
			Branches ["single"] = Stub;
			Branches ["iterator"] = Stub;
			Branches ["successful"] = Stub;

			InitializeConnection(
					modSettings ["host"], modSettings ["db"], 			                     
					modSettings ["user"], modSettings ["pass"],
					(bool)modSettings.GetBool("pool", true));

			InitializeQuery(modSettings["query"],
			             modSettings.Get("params", null) as List<object>);
		}

		/// <summary>
		/// Initializes the connection.
		/// </summary>
		/// <param name='databasehost'>
		/// Databasehost.
		/// </param>
		/// <param name='databasename'>
		/// Databasename.
		/// </param>
		/// <param name='user'>
		/// User.
		/// </param>
		/// <param name='password'>
		/// Password.
		/// </param>
		/// <param name='usepool'>
		/// Usepool.
		/// </param>
		private void InitializeConnection (string databasehost, string databasename, string user, string password, bool usepool)
		{
			ConnectionString = string.Format (ConnectionStringTemplate, databasehost, databasename, user, password, usepool);

			Connection = new MySqlConnection (ConnectionString);
			Connection.Open ();
		}

		/// <summary>
		/// Prepares the query.
		/// </summary>
		/// <param name='QueryTextFile'>
		/// Query text file.
		/// </param>
		/// <param name='queryParameters'>
		/// Query parameters.
		/// </param>
		private void InitializeQuery (string QueryTextFile, List<object> queryParameters = null)
		{
			this.QueryText = File.ReadAllText(QueryTextFile);

			this.OrderedParameters = new List<string> ();

			if (queryParameters != null) 
				foreach(object p in queryParameters) 
					this.OrderedParameters.Add((string)p);
		}

		/// <summary>
		/// Handles the branch changed.
		/// </summary>
		/// <param name='sender'>
		/// Sender.
		/// </param>
		/// <param name='e'>
		/// E.
		/// </param>
		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "none") none = e.NewValue;
			if (e.Name == "single") single = e.NewValue;
			if (e.Name == "iterator") iterator = e.NewValue;
			if (e.Name == "successful") successful = e.NewValue;
		}

		private IDbCommand GetCommand ()
		{
			IDbCommand Command = Connection.CreateCommand ();
			Command.CommandText = QueryText;
			Command.Prepare ();

			return Command;
		}

		private IDbDataParameter SetCommandParameter (IDbCommand command, string name, string value)
		{
			IDbDataParameter parameter = command.CreateParameter();
			parameter.ParameterName = name;
			parameter.Value = parameter[name];
			command.Parameters.Add(parameter);
		}

		private IDataReader ExecuteParameterizedCommand (IInteraction parameters)
		{
			IDbCommand Command = this.GetCommand();

			foreach (string paramname in OrderedParameters) 
				SetCommandParameter(Command, paramname, parameters[paramname]);

			return Command.ExecuteReader ();
		}

		private ResultInteraction GetResultInteraction (IDataReader reader, IInteraction parameters, ref int resultCount)
		{
			ResultInteraction resultline = none;

			if (reader.Read ()) {
				resultline = new ResultInteraction(parameters, reader);
				resultCount++;
			}
		}

		private bool GetResultsToBranches (IInteraction parameters)
		{			
			int resultCount = 0;
			bool success = true;
			IDataReader 
				reader = ExecuteParameterizedCommand (parameters);
			ResultInteraction 
				firstResult = GetResultInteraction (reader, parameters, ref resultCount),
			 	nextResult = GetResultInteraction(reader, parameters, ref resultCount);

			if (resultCount == 0) return none.TryProcess (parameters);
			if ((resultCount == 1) && (single != Stub)) return single.TryProcess(firstResult);

			success &= iterator.TryProcess(firstResult);

			while(resultCount > 1) {
				success &= iterator.TryProcess(nextResult);
				resultCount--;
				nextResult = GetResultInteraction(reader, parameters, ref resultCount);
			}

			return success;
		}

		protected override bool Process (IInteraction parameters)
		{
			if (GetResultsToBranches(parameters))
				return successful.TryProcess (parameters);
		}
	}
}

