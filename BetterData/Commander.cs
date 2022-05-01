using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using System.Data;
using BorrehSoft.Utilities.Collections.Settings;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using BorrehSoft.Utilities.Log;
using System.Text.RegularExpressions;

namespace BetterData
{
	public abstract class Commander : SingleBranchService
	{
		public string DatasourceName { get; set; }
		public SqlSource SqlSource { get; set; }
        public string ConnectionString {
            get
            {
                return Connector.Find(DatasourceName).ConnectionString;
            }
        }

        public override void LoadDefaultParameters (string defaultParameter)
		{
			if (File.Exists (defaultParameter)) {
				Settings ["sqlfile"] = defaultParameter;
			}
		}

		string description;

		public override string Description {
			get { return description; }
		}

		protected override void Initialize (Settings settings)
		{
			this.DatasourceName = settings.GetString ("connection", settings.GetString("_select", "default"));

            if (settings.TryGetString ("sqlfile", out string sqlFileName)) {
                this.SqlSource = new SqlFileSource(sqlFileName);
				description = (new FileInfo (sqlFileName)).Name;
			} else {
				throw new Exception("Expected 'sqlfile'");
			}
		}

		IDbDataParameter CreateParameter (IDbCommand command, string name, object value)
		{
			IDbDataParameter parameter = command.CreateParameter ();
			parameter.ParameterName = name;
			parameter.Value = value;
			return parameter;
		}

        protected List<MySqlParameter> FillParameters(
            IInteraction parameters
        ) {
            var pc = new List<MySqlParameter>();
            foreach (string name in this.SqlSource.GetParameterNames())
                if (parameters.TryGetFallback(name, out object value))
                    pc.Add(new MySqlParameter(name, value));
            return pc;
        }
    }
}
