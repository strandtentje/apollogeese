using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.Module
{
	public class JumpException : Exception
	{
		public string BranchName {
			get;
			private set;
		}

		public JumpException (string branchname) : base(string.Format("Couldn't return to module-branch {0}", branchname))
		{
			this.BranchName = branchname;
		}
	}
}
