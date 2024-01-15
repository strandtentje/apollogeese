using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using System.Data;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections;
using MySqlConnector;

namespace BetterData
{
	public class Connector : SingleBranchService
	{
		string Name;

		public string ConnectionString { get; private set; }

		static Map<Connector> NamedConnectors = new Map<Connector>();

		public override string Description {
			get {
				return string.Format(
					"Definition for connector {0}", this.Name);
			}
		}

		public virtual string ConnectionStringTemplate { 
			get {
				return 
					"Server=localhost; " +
					"Database={0}; " +
					"User ID={0}; " +
					"Password={0}; " +
					"Pooling=true; " +
					"Allow User Variables=True";
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			Settings ["connectionstring"] = defaultParameter;
		}

		public virtual char ParameterSeparator { get { return '@'; } }

		protected override void Initialize (Settings settings)
		{
            this.Name = settings.GetString("name", settings.GetString("_select", "default"));
			this.ConnectionString = settings.GetString ("connectionstring");

			NamedConnectors [this.Name] = this;
		}

		public virtual IDbConnection GetNewConnection() {				
			return new MySqlConnection(
				this.ConnectionString);
		}

		public static IDbConnection Find (string name)
		{
			return NamedConnectors [name].GetNewConnection ();
		}

		public override void OnReady ()
		{
			WithBranch.TryProcess (new SimpleInteraction ());
		}
	}


}

