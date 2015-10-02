using System;
using System.Data;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;

namespace BetterData
{
	public class Reader : Commander
	{
		public override string Description {
			get {
				return string.Format (
					"Reader {0} using {1}", 
					QueryName, DatasourceName);
			}
		}

		public override string BranchPrefix {
			get {
				return "changed";
			}
		}

		Service ChangedMany {
			get {
				return (Branches ["changedmany"] ?? Stub);
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			IDbCommand command = GetCommand (parameters);
			int affectedRows = command.ExecuteNonQuery ();

			bool success;

			success &= FindBranch(affectedRows).TryProcess (parameters);

			if (affectedRows > 1) {
				success &= ChangedMany.TryProcess (parameters);
			}

			return success;
		}
	}
}

