using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using L = BorrehSoft.Utensils.Log.Secretary;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using BorrehSoft.Utensils.Log;
using System.Text;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Databases
{
	/// <summary>
	/// Querier; queries.
	/// </summary>
	public abstract class DataQuerier : Service
	{
		public DataQuerier(IQueryConnection connection) {
			this.Connection = connection;
		}

		private string queryFile = "", queryText = "";
						
		public override string Description {
			get {
				string[] segments = queryFile.Split('/');
				return string.Format(segments[segments.Length - 1]);
			}
		}

		/// <summary>
		/// Gets the connection.
		/// </summary>
		/// <value>
		/// The connection.
		/// </value>
		public IQueryConnection Connection { get; private set; }

		public override void LoadDefaultParameters (string defaultParameter)
		{
			this.Settings ["query"] = defaultParameter;
		}

		[Instruction("File containing the SQL-script to execute.")]
		public string QueryFile {
			get {
				return this.queryFile;
			}
			set {
				this.queryFile = value;
				this.queryText = File.ReadAllText (this.queryFile);
			}
		}

		[Instruction("Names of parameters that should be passed into query.")]
		public List<string> Params { get; set; }

		[Instruction("Default values for parameters, if they're not available in context.")]
		public Settings Defaults { get; set; }

		public virtual string QueryText { 
			get {
				return this.queryText; 
			}
		}


		protected override void Initialize (Settings modSettings)
		{
			if (modSettings.Has ("query")) {
				this.QueryFile = modSettings.GetString ("query");
			} else if (modSettings.Has("querytext")) {
				this.queryText = modSettings.GetString ("querytext");
			}

			this.Params = modSettings.GetStringList ("params");
			this.Defaults = modSettings.GetSubsettings ("defaults");

			Connection.SetDefaultCommandQuery(queryText, this.Params);
		}
			
		/// <summary>
		/// Executes a parameterized command.
		/// </summary>
		/// <returns>
		/// The parameterized command.
		/// </returns>
		/// <param name='parameters'>
		/// Parameters.
		/// </param>
		private IDataReader ExecuteParameterizedCommand (IInteraction parameters)
		{
			IDataReader reader = null;

			try {
				reader = Connection.RunCommand(delegate(IQueryCommand command) {					
					object paramvalue;

					foreach (string paramname in Connection.DefaultOrderedParameters) {
						if (parameters.TryGetFallback (paramname, out paramvalue) || Defaults.TryGetValue(paramname, out paramvalue))
							command.SetParameter (paramname, paramvalue);
						else
							throw new Exception (string.Format("Parameter {0} not in interaction or defaults", paramname));
					}
				});
			} catch(Exception ex) {
				throw new Exception (
					string.Format ("Running command with {0} parameters failed", Connection.DefaultOrderedParameters.Count), ex);					
			}

			return reader;
		}

		Semaphore onConnection = new Semaphore(1 ,1);

		public abstract IQueryStats ConsumeResults (IInteraction parent, IDataReader reader);

		/// <summary>
		/// Gets the results to branches.
		/// </summary>
		/// <returns>
		/// True if all branches executed succesfully
		/// </returns>
		/// <param name='ParentParameters'>
		/// Parent parameters
		/// </param>
		/// <param name='IterateResultsBranchDelegate'>
		/// Process method for result iteration
		/// </param>
		/// <param name='SingleResultBranchDelegate'>
		/// Process method for single result
		/// </param>
		/// <param name='NoResultBranchDelegate'>
		/// Process method for no results; will use parentparameters
		/// </param>
		protected virtual IQueryStats AttemptQuery (IInteraction ParentParameters)
		{	
			IQueryStats stats;
		
			if (!onConnection.WaitOne (1000)) {
				throw new QueryException ("connection already occupied and waiting took too long");
			}

			try {
				using(IDataReader reader = ExecuteParameterizedCommand (ParentParameters)) {
					stats = ConsumeResults(reader);
				}
			} finally {
				onConnection.Release ();
			}

			return stats;
		}

		private string GetSignature(INosyInteraction parameters) {
			if (parameters.IncludeContext) {
				StringBuilder signatureBuilder = new StringBuilder ();
				object value;

				signatureBuilder.AppendLine (queryText);

				foreach (string paramName in Connection.DefaultOrderedParameters) {
					if (parameters.TryGetFallback (paramName, out value)) {
						signatureBuilder.Append (paramName);
						signatureBuilder.AppendLine (value.ToString ());
					}
				}

				return signatureBuilder.ToString ();
			} else {
				return queryFile;
			}
		}


		protected override bool Process (IInteraction parameters)
		{
			if (parameters is INosyInteraction) {
				INosyInteraction interaction = (INosyInteraction)parameters;

				interaction.Signature = GetSignature (interaction);

				return true;
			} else {
				return AttemptQuery (parameters).Successful && successful.TryProcess (parameters);
			}
		}

		internal bool ProcessDiscretely(IInteraction parameters) {
			return Process (parameters);
		}

		public override void Dispose ()
		{
			base.Dispose ();
			this.Connection.Connection.Close ();
		}
	}
}
