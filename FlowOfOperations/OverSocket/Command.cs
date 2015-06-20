using System;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket
{
	public class Command
	{
		public const int Type = 1;
		public const int String = 2;
		public const int Compose = 4;
		public const int Fallback = 8;
	}
}
