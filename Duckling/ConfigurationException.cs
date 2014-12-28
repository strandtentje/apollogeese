using System;

namespace BorrehSoft.ApolloGeese.Duckling
{
	public class ConfigurationException : Exception
	{
		public string BranchName { get; private set; }

		public ConfigurationException (string branchname) : base(string.Format("Missing setting {0}", branchname))
		{
			this.BranchName = branchname;
		}
	}
}

