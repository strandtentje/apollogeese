using System;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public class UnclonableException : Exception
	{
		public UnclonableException () : base("Cannot clone interaction. Are you trying to cache a ReachIn")
		{
		}
	}
}

