using System;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public enum Command
	{
		Type = 1,
		String = 2,
		Compose = 4,
		Fallback = 8,
	}
}
