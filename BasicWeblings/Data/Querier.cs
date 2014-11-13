using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Extensions.BasicWeblings.Server;
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
		private Service none, single, iterator, successful;
		private bool useAffectedRowcount;
		private string queryFile = "";

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

		protected abstract IQueryConnection CreateConnection(Settings modSettings);

		protected override void Initialize (Settings modSettings)
		{
			Branches ["none"] = Stub;
			Branches ["single"] = Stub;
			Branches ["iterator"] = Stub;
			Branches ["successful"] = Stub;

			Connection = CreateConnection(modSettings);

			queryFile = (string)modSettings["query"];
			Connection.SetDefaultCommandQuery(queryFile, modSettings.Get("params", null) as List<object>);

			useAffectedRowcount = modSettings.GetBool("useaffectedrowcount", false);

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
				if (parameters.TryGetFallback (paramname, out paramvalue))
					Command.SetParameter (paramname, paramvalue);
				else
					throw new Exception (string.Format("Parameter {0} not in interaction", paramname));
			}

			return Command.Run ();
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
		protected virtual bool BranchForMultipleResults (Func<ResultInteraction, bool> IteratorDelegate, ResultInteraction FirstResult, ResultInteraction NextResult, IDataReader DataReader, IInteraction ParentParameters)
		{	
			bool success = true;

			if (FirstResult != null) {
				success &= IteratorDelegate (FirstResult);

				if (NextResult != null) {
					int resultCount = 2;

					while (resultCount > 1) {
						success &= IteratorDelegate (NextResult);
						resultCount--;
						NextResult = GetResultInteraction (DataReader, ParentParameters, ref resultCount);
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
		protected bool GetResultsToBranches (IInteraction ParentParameters, 
		                                     Func<ResultInteraction, bool> IterateResultsBranchDelegate,
		                                     Func<ResultInteraction, bool> SingleResultBranchDelegate,
		                                     Func<IInteraction, bool> NoResultBranchDelegate)
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

			if ((resultCount == 0) && (NoResultBranchDelegate != Stub.TryProcess)) 
				success = NoResultBranchDelegate (ParentParameters);

			else if ((resultCount == 1) && (SingleResultBranchDelegate != Stub.TryProcess)) 
				success = SingleResultBranchDelegate (firstResult);	

			else if (IterateResultsBranchDelegate != Stub.TryProcess)
				success = BranchForMultipleResults(IterateResultsBranchDelegate, firstResult, nextResult, reader, ParentParameters);

			reader.Close();

			return success;
		}

		protected override bool Process (IInteraction parameters)
		{
			return GetResultsToBranches(parameters, iterator.TryProcess, single.TryProcess, none.TryProcess) && 
				successful.TryProcess (parameters);
		}
	}
}
