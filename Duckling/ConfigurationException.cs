using System;

namespace BorrehSoft.ApolloGeese.Duckling
{
	/// <summary>
	/// Configuration exception.
	/// </summary>
	public class ConfigurationException : Exception
	{
		public string BranchName { get; private set; }

		public ConfigurationException (string branchname) : base(string.Format("Missing setting {0}", branchname))
		{
			this.BranchName = branchname;
		}
	}
}

