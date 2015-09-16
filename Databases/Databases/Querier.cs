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
	public abstract class Querier : Service
	{
		private Service none, single, iterator, successful, capreached, noneAffected, oneAffected, someAffected, first;
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

		/// <summary>
		/// Creates a connection.
		/// </summary>
		/// <returns>A connection.</returns>
		/// <param name="modSettings">Connection settings</param>
		protected abstract IQueryConnection CreateConnection(Settings modSettings);

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

		[Instruction("Programmatically imposed limit on amount of result rows.", -1)]
		public int ResultCap { get; set; }

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
			Branches ["none"] = Stub;
			Branches ["single"] = Stub;
			Branches ["first"] = Stub;
			Branches ["iterator"] = Stub;
			Branches ["successful"] = Stub;
			Branches ["capreached"] = Stub;
			Branches ["noneaffected"] = Stub;
			Branches ["oneaffected"] = Stub;
			Branches ["someaffected"] = Stub;

			Connection = CreateConnection(modSettings);

			if (modSettings.Has ("query")) {
				this.QueryFile = modSettings.GetString ("query");
			} else if (modSettings.Has("querytext")) {
				this.queryText = modSettings.GetString ("querytext");
			}

			this.ResultCap = modSettings.GetInt ("resultcap", -1);
			this.Params = modSettings.GetStringList ("params");
			this.Defaults = modSettings.GetSubsettings ("defaults");

			Connection.SetDefaultCommandQuery(queryText, this.Params);

			if (modSettings.GetBool("runonce", false))
				Connection.GetDefaultCommand().Run().Close();
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
			if (e.Name == "first") first = e.NewValue;
			if (e.Name == "single") single = e.NewValue;
			if (e.Name == "iterator") iterator = e.NewValue;
			if (e.Name == "successful") successful = e.NewValue;
			if (e.Name == "capreached")	capreached = e.NewValue;
			if (e.Name == "noneaffected")
				noneAffected = e.NewValue;
			if (e.Name == "oneaffected")
				oneAffected = e.NewValue;
			if (e.Name == "someaffected")
				someAffected = e.NewValue;
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
			IQueryCommand Command = Connection.GetDefaultCommand ();

			object paramvalue;

			foreach (string paramname in Connection.DefaultOrderedParameters) {
				if (parameters.TryGetFallback (paramname, out paramvalue) || Defaults.TryGetValue(paramname, out paramvalue))
					Command.SetParameter (paramname, paramvalue);
				else
					throw new Exception (string.Format("Parameter {0} not in interaction or defaults", paramname));
			}

			try {
				return Command.Run ();
			} catch (Exception ex) {
				if (ex is MySqlException) 
					if (((MySqlException)ex).Number == 1054)
						throw new Exception (
							"SQL command failure; there's likely a malreference to the database within the query. Review query.",
							ex);
				
				throw new Exception (
					string.Format ("Running command with {0} parameters failed", Connection.DefaultOrderedParameters.Count), ex);					
			}

		}

		/// <summary>
		/// Gets a result interaction.
		/// </summary>
		/// <returns>
		/// Result interaction.
		/// </returns>
		/// <param name='reader'>
		/// Reader.
		/// </param>
		/// <param name='parameters'>
		/// Parameters.
		/// </param>
		/// <param name='resultCount'>
		/// Result count.
		/// </param>
		protected virtual ResultInteraction GetResultInteraction (IDataReader reader, IInteraction parameters, ref int resultCount)
		{
			ResultInteraction resultline = null;

			if (reader.Read ()) {
				resultline = new ResultInteraction(parameters, reader);
				resultCount++;
			}

			return resultline;
		}

		/// <summary>
		/// Branch for multiple results. 
		/// </summary>
		/// <returns>
		/// Succesful branch executiong
		/// </returns>
		/// <param name='IteratorDelegate'>
		/// Callback to iterate with
		/// </param>
		/// <param name='FirstResult'>
		/// Result that was first acquired.
		/// </param>
		/// <param name='NextResult'>
		/// Result that was acquired after that
		/// </param>
		/// <param name='DataReader'>
		/// Reader to acquire the remainder of the results from.
		/// </param>
		/// <param name='ParentParameters'>
		/// Parameters passed down from parent node.
		/// </param>
		protected virtual bool BranchForMultipleResults (ResultInteraction FirstResult, ResultInteraction NextResult, IDataReader DataReader, IInteraction ParentParameters)
		{	
			// squint your eyes at this code
			// or just sit at a distance
			// or take your glasses off
			//
			// can you see it
			// can you tell by the shape?
			// does the shape already give away that this bit of code
			// probably contains no less than three exploits?
			//
			// what an ugly, disastrous fuckwagon of source.

			bool success = true;

			if (FirstResult != null) {
				success &= first.TryProcess (FirstResult);
				success &= iterator.TryProcess (FirstResult);

				if (NextResult != null) {
					int resultCount = 2;
					int totalResults = 2;

					if ((ResultCap > 1) || (ResultCap < 0)) {
						while (resultCount > 1) {
							success &= iterator.TryProcess (NextResult);
							resultCount--;

							if ((ResultCap > -1) && (totalResults >= ResultCap)) {
								resultCount = 0;
								success &= capreached.TryProcess (ParentParameters);
							} else {
								NextResult = GetResultInteraction (DataReader, ParentParameters, ref resultCount);
								totalResults++;
							}
						}
					}
				}
			}

			return success;
		}

		Semaphore onConnection = new Semaphore(1 ,1);

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
		protected bool GetResultsToBranches (IInteraction ParentParameters)
		{			
			int resultCount, recordsAffected; 
			bool success;
			IDataReader reader = null;
			ResultInteraction firstResult, nextResult;

			resultCount = 0;

			if (!onConnection.WaitOne (1000)) {
				throw new QueryException ("connection already occupied and waiting took too long");
			}

			try {
				using(reader = ExecuteParameterizedCommand (ParentParameters)) {
					success = true;

					recordsAffected = reader.RecordsAffected;

					firstResult = GetResultInteraction (reader, ParentParameters, ref resultCount);
					nextResult = GetResultInteraction (reader, ParentParameters, ref resultCount);

					if ((resultCount > 1) && (iterator != Stub)) success = BranchForMultipleResults(firstResult, nextResult, reader, ParentParameters);
				}
			} finally {
				onConnection.Release ();
			}

			if (recordsAffected == 0)
				success &= noneAffected.TryProcess (ParentParameters);
			if (recordsAffected == 1)
				success &= oneAffected.TryProcess (ParentParameters);
			if (recordsAffected > 1)
				success &= someAffected.TryProcess (ParentParameters);

			if ((resultCount == 0) && (none != Stub)) success = none.TryProcess (ParentParameters);

			else if ((resultCount == 1) && (single != Stub)) success = single.TryProcess (firstResult);	

			return success;
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
				return GetResultsToBranches (parameters) && successful.TryProcess (parameters);
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
