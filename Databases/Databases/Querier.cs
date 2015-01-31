using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using L = BorrehSoft.Utensils.Log.Secretary;
using BorrehSoft.ApolloGeese.Duckling.Http;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace BorrehSoft.Extensions.BasicWeblings.Data
{
	/// <summary>
	/// Querier; queries.
	/// </summary>
	public abstract class Querier : Service
	{
		private Service none, single, iterator, successful, capreached;
		private bool useAffectedRowcount;
		private int resultCap = -1;
		private string queryFile = "", queryText = "";
		private Settings defaultParameters;

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

		protected override void Initialize (Settings modSettings)
		{
			Branches ["none"] = Stub;
			Branches ["single"] = Stub;
			Branches ["iterator"] = Stub;
			Branches ["successful"] = Stub;
			Branches ["capreached"] = Stub;

			Connection = CreateConnection(modSettings);

			queryFile = modSettings.GetString("query", "");
			queryText = modSettings["querytext"] as String ?? File.ReadAllText(queryFile);
			if (modSettings.Has ("resultcap")) {
				resultCap = (int)modSettings["resultcap"];
			}

			Connection.SetDefaultCommandQuery(queryText, modSettings.Get("params", null) as List<object>);

			useAffectedRowcount = modSettings.GetBool("useaffectedrowcount", false);

			defaultParameters = modSettings.GetSubsettings ("defaults");

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
			if (e.Name == "single") single = e.NewValue;
			if (e.Name == "iterator") iterator = e.NewValue;
			if (e.Name == "successful") successful = e.NewValue;
			if (e.Name == "capreached")	capreached = e.NewValue;
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
				if (parameters.TryGetFallback (paramname, out paramvalue) || defaultParameters.TryGetValue(paramname, out paramvalue))
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
			bool success = true;

			if (FirstResult != null) {
				success &= iterator.TryProcess (FirstResult);

				if (NextResult != null) {
					int resultCount = 2;
					int totalResults = 2;

					while (resultCount > 1) {
						success &= iterator.TryProcess (NextResult);
						resultCount--;

						if ((resultCap > -1) && (totalResults >= resultCap)) {
							resultCount = 0;
							success &= capreached.TryProcess (ParentParameters);
						} else {
							NextResult = GetResultInteraction (DataReader, ParentParameters, ref resultCount);
							totalResults++;
						}
					}
				}
			}

			return success;
		}

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
			int resultCount; 
			bool success;
			IDataReader reader;
			ResultInteraction firstResult, nextResult;

			resultCount = 0;
			reader = ExecuteParameterizedCommand (ParentParameters);

			if (useAffectedRowcount) {
				return reader.RecordsAffected > 0;
			}

			firstResult = GetResultInteraction (reader, ParentParameters, ref resultCount);
			nextResult = GetResultInteraction (reader, ParentParameters, ref resultCount);

			success = true;

			if ((resultCount > 1) && (iterator != Stub)) success = BranchForMultipleResults(firstResult, nextResult, reader, ParentParameters);

			reader.Close();

			if ((resultCount == 0) && (none != Stub)) success = none.TryProcess (ParentParameters);

			else if ((resultCount == 1) && (single != Stub)) success = single.TryProcess (firstResult);	

			return success;
		}

		protected override bool Process (IInteraction parameters)
		{
			return GetResultsToBranches(parameters) && successful.TryProcess (parameters);
		}

		public override void Dispose ()
		{
			base.Dispose ();
			this.Connection.Connection.Close ();
		}
	}
}
