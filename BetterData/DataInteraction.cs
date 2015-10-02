using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Data;
using System.Collections.Generic;

namespace BetterData
{
	class DataInteraction : SimpleInteraction
	{
		string[] ColumnNames;

		object[] CellValues;

		public DataInteraction (
			IInteraction parameters, 
			string[] columnNames, 
			object[] values) : base(
				parameters)
		{
			this.ColumnNames = columnNames;
			this.CellValues = values;
		}

		public override bool Has (string key)
		{
			bool hasColumn = Array.IndexOf (ColumnNames, key) > -1;

			return hasColumn || base.Has (key);
		}

		public override object Get (string key)
		{
			int columnIndex = Array.IndexOf (ColumnNames, key);

			if (columnIndex < 0) {
				return base.Get (key);
			} else {
				return CellValues [columnIndex];
			}
		}

	}

}

