using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;

namespace InputProcessing
{
	class UrlKeyValueReader : IKeyValueReader, SimpleInteraction
	{
		string combinedSourcename;

		public UrlKeyValueReader (IInteraction parent, TextReader dataReader, string sourceName) : base(parent)
		{
			this.combinedSourcename = string.Format ("url-encoded:{0}", sourceName);
		}

		string SourceName { get { return combinedSourcename; } }

		Stream IncomingBody { get; }

		/// <summary>
		/// Gets a StreamReader for the underlying bodystream.
		/// </summary>
		/// <returns>The incoming body reader.</returns>
		StreamReader GetIncomingBodyReader();

		/// <summary>
		/// Gets a value indicating whether or not a reader has been produced for the stream
		/// </summary>
		/// <returns><c>true</c> if this instance has a reader; otherwise, <c>false</c>.</returns>
		bool HasReader();

		bool TryGetName (out string name);

		void FinalizeValidation ();
	}

}

