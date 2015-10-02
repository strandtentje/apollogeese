using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using System.Data;

namespace BetterData
{
	public abstract class NBranch : Service
	{
		public abstract string BranchPrefix { get; }

		protected Service FindBranch (int affectedRows)
		{
			string branchName;
			branchName = string.Format (
				"{0}{1}", this.BranchPrefix, affectedRows);

			return (Branches [branchName] ?? Stub);
		}
	}


}

