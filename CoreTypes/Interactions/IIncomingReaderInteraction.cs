using System;
using System.IO;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
		public interface IIncomingReaderInteraction : IInteraction
	{
		/// <summary>
		/// Gets a StreamReader for the underlying bodystream.
		/// </summary>
		/// <returns>The incoming body reader.</returns>
		TextReader GetIncomingBodyReader();

		/// <summary>
		/// Gets a value indicating whether or not a reader has been produced for the stream
		/// </summary>
		/// <returns><c>true</c> if this instance has a reader; otherwise, <c>false</c>.</returns>
		bool HasReader();
	}
}

