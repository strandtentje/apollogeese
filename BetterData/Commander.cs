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
	public abstract class Commander : SingleBranchService
	{
		private string DSN;

		private bool AutoSelectLastInsertId;

		IDbConnection connection;
		DateTime timestamp;

		/// <summary>
		/// Gets the connection.
		/// </summary>
		/// <value>
		/// The connection.
		/// </value>
		public IDbConnection Connection { 
			get {
					DateTime nowish = DateTime.Now;
				TimeSpan dif = (nowish - timestamp);

				bool 
				noconnection = connection == null,
				notopen = noconnection || (connection.State != ConnectionState.Open),
				tooold = dif.TotalMinutes > 1;

				timestamp = nowish;

				if (notopen || tooold) {
					Secretary.Report (5, "Reviving MySQL connection because it was:", (notopen ? "not open" : ""), (tooold ? "too old" : ""));

					if (connection != null) {
						try {
							connection.Dispose();
						} catch(Exception ex) {
							Secretary.Report (5, "Failed to dispose of old one due to:", ex.Message);
						}
					}

					connection = Connector.Find (this.DatasourceName);
					connection.Open ();
				}

				return connection;
			}
		}

		public string DatasourceName { 
			get {
				return this.DSN; 
			}
			set {
				this.connection = null;
				this.DSN = value;
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
				this.sqlSource = new AutoSqlFileSource(value, AutoSelectLastInsertId);
			}
		}

		string description;

		public override string Description {
			get { return description; }
		}

		protected override void Initialize (Settings settings)
		{
			this.DatasourceName = settings.GetString ("connection", "default");
            this.UseTransaction = settings.GetBool("usetransaction", false);
			this.AutoSelectLastInsertId = settings.GetBool ("lastinsertid", false);

			if (settings.Has ("sql")) {
				this.SqlText = settings.GetString ("sql");
				description = this.SqlText;
			} else if (settings.Has ("sqlfile")) {
				this.SqlFile = settings.GetString ("sqlfile");
				description = (new FileInfo (this.SqlFile)).Name;
			} else if (settings.Has ("autosql")) {
				this.AutoSqlFile = settings.GetString ("autosql");
				description = (new FileInfo (this.AutoSqlFile)).Name;
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

        protected void UseCommand(IInteraction parameters, Action<IDbCommand> callback, IDbCommand command)
        {
            command.CommandText = this.sqlSource.GetText();

            foreach (string name in this.sqlSource.GetParameterNames())
            {
                object value;
                if (parameters.TryGetFallback(name, out value))
                {
                    command.Parameters.Add(CreateParameter(command, name, value));
                }
            }

            callback(command);
        }

        private IDbConnection GetConnection(IInteraction parameters)
        {
            if (this.UseTransaction)
            {
                IInteraction candidate;
                TransactionInteraction transaction;

                while((parameters != null) && parameters.TryGetClosest(typeof(TransactionInteraction), out candidate)) {
                    parameters = candidate.Parent;
                    transaction = (TransactionInteraction)candidate;
                    if (transaction.DatasourceName == this.DatasourceName)
                    {
                        return transaction.Connection;
                    }
                }
            }

            return Connection;
        }

		protected void UseCommand(IInteraction parameters, Action<IDbCommand> callback) {
            IDbConnection connection = GetConnection(parameters);

			lock (connection) {
				using (IDbCommand command = connection.CreateCommand ()) {
                    UseCommand(parameters, callback, command);
				}
			}
		}


        public bool UseTransaction { get; set; }
    }
}
