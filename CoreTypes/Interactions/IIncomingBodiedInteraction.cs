using System;
using System.IO;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	/// <summary>
	/// Interaction for Services which can read from upstream streams.
	/// </summary>
	public interface IIncomingBodiedInteraction : IIncomingReaderInteraction
	{
		string SourceName { get; }

		Stream IncomingBody { get; }

	}
}

