using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Data;
using System.Collections.Generic;

namespace BetterData
{
	class DataInteraction : SimpleInteraction
	{
		public DataInteraction (
			IInteraction parameters, 
			string[] columnNames, 
			object[] values) : base(
				parameters)
		{
			for (int columnIndex = 0; columnIndex < columnNames.Length; columnIndex++) {
				this [columnNames [columnIndex]] = values [columnIndex];
			}
		}
	}

}

