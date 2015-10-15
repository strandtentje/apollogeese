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
using System.Text.RegularExpressions;

namespace BetterData
{
	public abstract class Commander : Service
	{
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

		private SqlSource sqlSource;

		public override void LoadDefaultParameters (string defaultParameter)
		{
			if (File.Exists (defaultParameter)) {
				Settings ["sqlfile"] = defaultParameter;
			} else if (defaultParameter.ToLower().EndsWith (".auto.sql")) {
				Settings ["autosql"] = defaultParameter;
			} else {
				Settings ["sql"] = defaultParameter;
			}
		}

		string sqlFile;

		string SqlText {
			get { 
				return this.sqlSource.GetText ();
			} set {
				this.sqlSource = new SqlLiteralSource (value);
			}
		}

		string SqlFile {
			get {
				return this.sqlFile;
			} set {
				this.sqlFile = value;
				this.sqlSource = new SqlFileSource (value);
			}
		}

		string AutoSqlFile {
			get {
				return this.sqlFile;
			}
			set {
				this.sqlFile = value;
				this.sqlSource = new AutoSqlFileSource(value);
			}
		}

		protected override void Initialize (Settings settings)
		{
			this.DatasourceName = settings.GetString ("connection", "default");

			if (settings.Has ("sql")) {
				this.SqlText = settings.GetString ("sql");
			} else if (settings.Has ("sqlfile")) {
				this.SqlFile = settings.GetString ("sqlfile");
			} else if (settings.Has ("autosql")) {
				this.AutoSqlFile = settings.GetString ("autosql");
			} else {
				throw new Exception("Expected either 'sql', 'sqlfile' or 'autosql'");
			}
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
					command.CommandText = this.sqlSource.GetText();

					foreach (string name in this.sqlSource.GetParameterNames()) {
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
