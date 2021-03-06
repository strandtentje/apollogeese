using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	/// <summary>
	/// Incoming interaction for HTTP response
	/// </summary>
	public class SimpleIncomingInteraction : SimpleInteraction, IIncomingBodiedInteraction
	{		
		private StreamReader reader = null;

		/// <summary>
		/// Gets the incoming body of data
		/// </summary>
		/// <value>The incoming body.</value>
		public Stream IncomingBody { get; protected set; }

		/// <summary>
		/// Gets a streamreader to read from the incoming body of data
		/// </summary>
		/// <returns>The incoming body reader.</returns>
		public TextReader GetIncomingBodyReader() { 
			if (reader == null)
				reader = new StreamReader (IncomingBody);

			return reader;
		}

		/// <summary>
		/// Gets a value indicating whether or not a reader has been produced for the stream
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public bool HasReader()
		{
			return reader != null;
		}

		private string sourceName;

		public string SourceName {
			get {
				return sourceName;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Extensions.BasicWeblings.Client.HttpResponseInteraction"/> class using
		/// a stream of incoming body data.
		/// </summary>
		/// <param name="bodyStream">Body stream.</param>
		/// <param name="parent">Parent.</param>
		public SimpleIncomingInteraction (Stream bodyStream, IInteraction parent, string sourceName, string contentType) : base(parent)
		{
			this.sourceName = sourceName;
			this.ContentType = contentType;
			IncomingBody = bodyStream;
		}

		/// <summary>
		/// Gets the MIME-type associated with the underlying data
		/// </summary>
		/// <value>The type of the MIME.</value>
		public string ContentType { get; protected set; }

	}
}

