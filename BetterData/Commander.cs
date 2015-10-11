using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using System.Data;
using BorrehSoft.Utensils.Collections.Settings;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using BorrehSoft.Utensils.Log;

namespace BetterData
{
	public abstract class Commander : Service
	{
		List<string> ParameterNames;

		private string DSN;
		private IDbConnection Connection;

		public string DatasourceName { 
			get {
				return this.DSN; 
			}
			set {
				if (this.Connection != null) {
					try {
						this.Connection.Dispose();
					} catch (Exception ex) {
						Secretary.Report (5, "Disposing of old connection failed. " +
							"This shouldn't be *that* bad depending on what it says " +
							"down there:");
						Secretary.Report (5, ex.Message);
					}
				}

				this.DSN = value;
				this.Connection = Connector.Find (value);
			}
		}

		private ITextSource querySource;

		public override void LoadDefaultParameters (string defaultParameter)
		{
			if (File.Exists (defaultParameter)) {
				if (defaultParameter.EndsWith (".auto.sql")) {
					Settings ["generate"] = defaultParameter;
				} else {
					Settings ["queryfile"] = defaultParameter;
				}
			} else {
				Settings ["query"] = defaultParameter;
			}
		}

		string queryFile;

		string Query {
			get { 
				return this.querySource.GetText ();
			} set {
				this.querySource = new PlainTextSource (value);
			}
		}

		string QueryFile {
			get {
				return this.queryFile;
			} set {
				this.queryFile = value;
				this.querySource = new TextFileSource (value);
			}
		}

		string GenerateFile {
			get {
				return this.queryFile;
			}
			set {
				this.queryFile = value;
				this.querySource = new GeneratedSqlTextFile(value);
			}
		}

		protected override void Initialize (Settings settings)
		{
			this.DatasourceName = settings.GetString ("connection", "default");

			if (settings.Has ("query")) {
				this.Query = settings.GetString ("query");
			} else if (settings.Has ("queryfile")) {
				this.QueryFile = settings.GetString ("queryfile");
			} else if (settings.Has ("generate")) {
				this.GenerateFile = settings.GetString ("generate");
			} else {
				throw new Exception("Expected either 'query', 'queryfile' or 'generate'");
			}

			this.ParameterNames = settings.GetStringList ("params");
		}

		IDbDataParameter CreateParameter (IDbCommand command, string name, object value)
		{
			IDbDataParameter parameter = command.CreateParameter ();
			parameter.ParameterName = name;
			parameter.Value = value;
			return parameter;
		}

		protected void UseCommand(IInteraction parameters, Action<IDbCommand> callback) {
			// using idb-commands within a lock. how kinky.
			// i guess you could call this
			// **puts on sunglasses**
			// DBSM

			lock (Connection) {
				using (IDbCommand command = Connection.CreateCommand ()) {
					command.CommandText = this.querySource.GetText ();

					foreach (string name in this.ParameterNames) {
						object value;
						if (parameters.TryGetFallback (name, out value)) {
							command.Parameters.Add (CreateParameter (command, name, value));
						}
					}

					callback (command);
				}
			}
		}	
	}
}
