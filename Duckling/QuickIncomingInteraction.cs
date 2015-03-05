using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.IO;

namespace BorrehSoft.ApolloGeese.Duckling
{
	/// <summary>
	/// Incoming interaction for HTTP response
	/// </summary>
	public class QuickIncomingInteraction : QuickInteraction, IIncomingBodiedInteraction
	{		
		private StreamReader writer = null;

		/// <summary>
		/// Gets the incoming body of data
		/// </summary>
		/// <value>The incoming body.</value>
		public Stream IncomingBody { get; private set; }

		/// <summary>
		/// Gets a streamreader to read from the incoming body of data
		/// </summary>
		/// <returns>The incoming body reader.</returns>
		public StreamReader GetIncomingBodyReader() { 
			if (writer == null)
				writer = new StreamReader (IncomingBody);

			return writer;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Extensions.BasicWeblings.Client.HttpResponseInteraction"/> class using
		/// a stream of incoming body data.
		/// </summary>
		/// <param name="bodyStream">Body stream.</param>
		/// <param name="parent">Parent.</param>
		public QuickIncomingInteraction (Stream bodyStream, IInteraction parent) : base(parent)
		{
			IncomingBody = bodyStream;
		}

		/// <summary>
		/// Gets a value indicating whether or not a reader has been produced for the stream
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public bool HasReader()
		{
			return writer != null;
		}
	}
}

