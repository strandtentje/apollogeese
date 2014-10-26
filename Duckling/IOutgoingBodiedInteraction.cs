using System;
using System.IO;

namespace BorrehSoft.ApolloGeese.Duckling
{
	public interface IOutgoingBodiedInteraction : IInteraction
	{
		StreamWriter OutgoingBody { get; }
	}
}

