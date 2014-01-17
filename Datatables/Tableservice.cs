using System;
using System.Data;
using MySql.Data.MySqlClient;
using BorrehSoft.ApolloGeese.Duckling;
using System.Net;
using BorrehSoft.Utensils.Settings;
using BorrehSoft.Utensils;
using System.Text.RegularExpressions;

namespace Datatables
{
	public class Tableservice : Service
	{
		private const string queryParameterRegex = "@([a-z|A-Z]*)\\w";
		private string query;
		private IDbConnection databaseConnection;
		private List<string> queryParameters = new List<string>();

		public override string[] AdvertisedBranches {
			get {
				return new string[] { "row" };
			}
		}

		public override string Description {
			get {
				return "Turns a query-result into an HTML table";
			}
		}
		
		protected override void Initialize (Settings modSettings)
		{
			string connectionString = string.Format (
				"Server={0};Database={1};User ID={2};Password={3};Pooling=false",
				(string)modSettings ["server"],
				(string)modSettings ["database"],
				(string)modSettings ["user"],
				(string)modSettings ["password"]);

			query = Regex.Replace ((string)modSettings ["query"], queryParameterRegex, 
				delegate(Match mn) {
					queryParameters.Add(mn.Captures[1].Value);
					return "?";	});

			databaseConnection = new MySqlConnection (connectionString);
			databaseConnection.Open ();
		}

		protected override bool Process (Interaction parameters)
		{
			IDbCommand command = databaseConnection.CreateCommand ();
			command.CommandText = query;

			foreach (string parameterName in queryParameters)
				command.Parameters.Add (parameters.Luggage [parameterName]);

			IDataReader queryReader = command.ExecuteReader ();

			while (queryReader.Read()) {
				for (int i = 0; i < queryReader.FieldCount; i++) {
					string fieldName = queryReader.GetName (i);
					parameters.Luggage [fieldName] = 
						queryReader [fieldName].ToString ();
				}

				RunBranch ("row", parameters);
			}

			queryReader.Close ();
			command.Dispose ();

			return true;
		}


	}
}