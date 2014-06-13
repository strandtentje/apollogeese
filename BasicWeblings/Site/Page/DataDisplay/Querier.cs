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
		private Service iterator = Stub;
		private Service successful = Stub;

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
			Branches ["iterator"] = Stub;
			Branches ["successful"] = Stub;

			ConnectionString = string.Format (ConnectionStringTemplate, 
			                                 modSettings ["host"],
			                                 modSettings ["db"],
			                                 modSettings ["user"],
			                                 modSettings ["pass"],
			                                 (bool)modSettings.GetBool("pool", true));

			Connection = new MySqlConnection (ConnectionString);

			Connection.Open ();

			QueryText = File.ReadAllText((string)modSettings ["query"]);

			OrderedParameters = new List<string> ();

			if (modSettings.Has ("params")) {
				List<object> parameters = (List<object>)modSettings ["params"];
				foreach(object p in parameters) OrderedParameters.Add((string)p);
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "iterator") iterator = e.NewValue;
			if (e.Name == "successful") successful = e.NewValue;
		}


		protected override bool Process (IInteraction parameters)
		{
			IDbCommand Command = Connection.CreateCommand ();
			Command.CommandText = QueryText;
			Command.Prepare ();

			foreach (string paramname in OrderedParameters) {
				IDbDataParameter parameter = Command.CreateParameter ();
				parameter.ParameterName = paramname;
				parameter.Value = parameters [paramname];
				Command.Parameters.Add (parameter);
			}

			IDataReader reader = Command.ExecuteReader();
			ResultInteraction resultline = null;

			bool success = true;

			while (reader.Read()) 
			{
				resultline = new ResultInteraction (parameters, reader);
				success &= iterator.TryProcess(resultline);
			}

			reader.Close();

			if (success && (successful != Stub)) successful.TryProcess(parameters);

			return success;
		}
	}
}

