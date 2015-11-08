using System;
using System.IO;
using System.Text;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	/// <summary>
	/// Interaction for services which will write to upstream streams.
	/// </summary>
	public interface IOutgoingBodiedInteraction : IInteraction
	{
		Stream OutgoingBody { get; }

		Encoding Encoding { get; }
	}
}

