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

namespace BetterData
{
	public abstract class Commander : Service
	{
		public string QueryName { 
			get { 
				return "";
			}
		}

		List<string> ParameterNames;

		public string DatasourceName { get; private set; }

		private ITextSource querySource;

		private ConnectionPool ConnectionPool;

		public override void LoadDefaultParameters (string defaultParameter)
		{
			if (File.Exists (defaultParameter)) {
				Settings ["queryfile"] = defaultParameter;
			} else {
				Settings ["query"] = defaultParameter;
			}
		}

		protected override void Initialize (Settings settings)
		{
			this.DatasourceName = settings.GetString ("connection", "default");
			this.ConnectionPool = new ConnectionPool (this.DatasourceName);

			if (settings.Has ("query")) {
				querySource = new PlainTextSource (settings.GetString ("query"));
			} else if (settings.Has ("queryfile")) {
				querySource = new TextFileSource (settings.GetString ("queryfile"));
			} else {
				throw new Exception ("Expected either 'query' or 'queryfile'");
			}

			this.ParameterNames = settings.GetStringList ("params");
		}

		private void GetCommand(IInteraction parameters) {


			if (querySource.IsChanged) {
				querySource.AcknowledgeChange ();

				if (residentCommand == null) {
					residentCommand = Connection.CreateCommand ();
				}

				residentCommand.CommandText = querySource.GetText ();
			}

			residentCommand.Parameters.Clear ();

			foreach (string parameterName in this.ParameterNames) {
				IDbDataParameter parameter = residentCommand.CreateParameter ();

				object paramValue; 
				if (parameters.TryGetFallback(parameterName, out paramValue)) {
					parameter.ParameterName = parameterName;
					parameter.Value = paramValue;
				}

				residentCommand.Parameters.Add (parameter);
			}
		}

		public void RunCommand(
			IInteraction parameters,
			Action<IDbCommand> callback) {

			callback (GetCommand(parameters));
		}
	}

}

