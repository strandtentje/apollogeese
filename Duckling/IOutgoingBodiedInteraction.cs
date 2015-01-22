using System;
using System.IO;

namespace BorrehSoft.ApolloGeese.Duckling
{
	public interface IOutgoingBodiedInteraction : IInteraction
	{
		Stream OutgoingBody { get; }

		StreamWriter GetOutgoingBodyWriter();

		/// <summary>
		/// Gets a value indicating whether or not a writer has been produced for the stream
		/// </summary>
		/// <returns><c>true</c> if this instance has writer; otherwise, <c>false</c>.</returns>
		bool HasWriter();
	}
}

