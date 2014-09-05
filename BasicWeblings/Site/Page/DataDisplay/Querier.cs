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

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page.DataDisplay
{
	/// <summary>
	/// Querier; queries.
	/// </summary>
	public class Querier : Service
	{
		private Service none, single, iterator, successful;

		public override string Description {
			get {
				return "Queries and iterates with attached service.";
			}
		}


		/// <summary>
		/// Gets the connection.
		/// </summary>
		/// <value>
		/// The connection.
		/// </value>
		public QueryConnection Connection { get; private set; }

		protected override void Initialize (Settings modSettings)
		{
			Branches ["none"] = Stub;
			Branches ["single"] = Stub;
			Branches ["iterator"] = Stub;
			Branches ["successful"] = Stub;

			Connection = new QueryConnection(
					(string)modSettings ["host"], (string)modSettings ["db"], 			                     
					(string)modSettings ["user"], (string)modSettings ["pass"],
					(bool)modSettings.GetBool("pool", true));


			Connection.SetDefaultCommandQuery((string)modSettings["query"],
			             modSettings.Get("params", null) as List<object>);
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
			QueryCommand Command = Connection.GetDefaultCommand();

			foreach (string paramname in Connection.DefaultOrderedParameters) 
				Command.SetParameter(paramname, parameters[paramname]);

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
		                                     Func<ResultInteraction, bool> IterateResultsBranchDelegate = null,
		                                     Func<ResultInteraction, bool> SingleResultBranchDelegate = null,
		                                     Func<IInteraction, bool> NoResultBranchDelegate = null)
		{			
			int resultCount; 
			bool success;
			IDataReader reader;
			ResultInteraction firstResult, nextResult;

			resultCount = 0;
			reader = ExecuteParameterizedCommand (ParentParameters);

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
