using System;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public class MissingBranchException : Exception
	{
		public MissingBranchException(string name) : base(
			string.Format("Branch {0} was expected but not found", 
				name)) {}
	}
}

