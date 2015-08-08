using System;
using System.IO;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	/// <summary>
	/// Interaction for services which will write to upstream streams.
	/// </summary>
	public interface IOutgoingBodiedInteraction : IInteraction
	{
		Stream OutgoingBody { get; }

		/// <summary>
		/// Gets a StreamWriter for the ougoing body.
		/// </summary>
		/// <returns>The outgoing body writer.</returns>
		StreamWriter GetOutgoingBodyWriter();

		/// <summary>
		/// Gets a value indicating whether or not a writer has been produced for the stream
		/// </summary>
		/// <returns><c>true</c> if this instance has writer; otherwise, <c>false</c>.</returns>
		bool HasWriter();
	}
}

