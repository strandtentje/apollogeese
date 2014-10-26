using System;
using System.IO;

namespace BorrehSoft.ApolloGeese.Duckling
{
	public interface IIncomingBodiedInteraction : IInteraction
	{
		StreamReader IncomingBody { get; }
	}
}

