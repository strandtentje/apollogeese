using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Data;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections;

namespace BetterData
{
	public class DataReader : Commander
	{
		public override string Description {
			get {
				return string.Format (
					"Read {1} from {0}", 
					DatasourceName,
					base.Description
				);
			}
		}

		IntMap<Service> rowBranches = new IntMap<Service>() { Default = Stub };

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			base.HandleBranchChanged (sender, e);

			rowBranches.Set (e.Name, e.NewValue, "row_");

			if (e.Name == "iterator") rowBranches.Default = e.NewValue;
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

		Service Last {
			get {
				return (Branches ["last"] ?? Stub);
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
			IInteraction lastRow = parameters;
			int currentRowNumber = -1;

			UseCommand (parameters, delegate(IDbCommand command) {
				using (IDataReader reader = command.ExecuteReader()) {
					string[] columnNames = GetColumnNames (reader);

					for (currentRowNumber = 0; reader.Read (); currentRowNumber++) {
						object[] values = new object[reader.FieldCount];

						for (int i = 0; i < reader.FieldCount; i++) {
							values[i] = reader.GetValue(i);
						}

						DataInteraction dataRow = new DataInteraction (
							parameters, columnNames, values);
						lastRow = dataRow;

						success &= rowBranches[currentRowNumber].TryProcess (dataRow);
					}
				}
			}); 

			if (currentRowNumber == 0)
				success &= None.TryProcess (parameters);
			else if (currentRowNumber == 1)
				success &= Single.TryProcess (lastRow); 

			if (currentRowNumber > 0)
				success &= Last.TryProcess (lastRow);

			return success;
		}
	}
}

