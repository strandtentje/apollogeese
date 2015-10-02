using System;
using System.Data;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;

namespace BetterData
{
	public class DataWriter : Commander
	{
		public override string Description {
			get {
				return string.Format (
					"Write with {0} on {1}", 
					QueryName, DatasourceName);
			}
		}

		public override string BranchPrefix {
			get {
				return "changed";
			}
		}

		protected override Service DefaultBranch {
			get {
				return Branches ["default"] ?? Stub;
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			IDbCommand command = GetCommand (parameters);

			int affectedRows = command.ExecuteNonQuery ();

			bool success;

			success &= FindBranch(affectedRows).TryProcess (parameters);

			return success;
		}
	}
}

