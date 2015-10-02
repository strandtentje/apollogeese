using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Data;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections.Maps;

namespace BetterData
{
	public class DataReader : Commander
	{
		public override string Description {
			get {
				return string.Format (
					"Read with {0} from {1}",
					QueryName, DatasourceName);
			}
		}

		BranchesByNumber rowBranches = new BranchesByNumber ("row");

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			base.HandleBranchChanged (sender, e);

			rowBranches.SetBranch (e.Name, e.NewValue);

			if (e.Name == "iterator") rowBranches.DefaultBranch = e.NewValue;
		}

		Service None {
			get {
				return (Branches ["none"] ?? Stub);
			}
		}

		Service Single {
			get {
				return (Branches ["single"] ?? Stub);
			}
		}

		string[] GetColumnNames (IDataReader reader)
		{
			string[] columnNames = new string[reader.FieldCount];

			for (int i = 0; i < reader.FieldCount; i++) 
				columnNames [i] = reader.GetName (i);

			return columnNames;
		}

		protected override bool Process (IInteraction parameters)
		{
			bool success = true;

			IDataReader reader = TakeCommand (parameters).ExecuteReader ();

			DataInteraction lastRow;

			string[] columnNames = GetColumnNames (reader);

			int currentRowNumber;

			for (currentRowNumber = 0; reader.Read (); currentRowNumber++) {
				object[] values = new object[reader.FieldCount];

				reader.GetValues (values);

				DataInteraction dataRow = new DataInteraction (parameters, columnNames, values);
				lastRow = dataRow;

				success &= rowBranches.Find (currentRowNumber).TryProcess (dataRow);
			}

			GiveCommand ();

			if (currentRowNumber == 0)
				success &= None.TryProcess (parameters);
			else if (currentRowNumber == 1)
				success &= Single.TryProcess (parameters);

			return success;
		}
	}
}

