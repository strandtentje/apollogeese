using System;
using System.IO;

namespace BorrehSoft.ApolloGeese.Duckling
{
	public interface IIncomingBodiedInteraction : IInteraction
	{
		Stream IncomingBody { get; }

		StreamReader GetIncomingBodyReader();

		/// <summary>
		/// Gets a value indicating whether or not a reader has been produced for the stream
		/// </summary>
		/// <returns><c>true</c> if this instance has a reader; otherwise, <c>false</c>.</returns>
		bool HasReader();
	}
}

