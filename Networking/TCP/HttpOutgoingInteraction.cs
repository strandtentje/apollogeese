using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.IO;

namespace BorrehSoft.Extensions.Networking.TCP
{
	/// <summary>
	/// Outgoing Interaction for HTTP communications
	/// </summary>
	public class HttpOutgoingInteraction :  QuickInteraction, IOutgoingBodiedInteraction
	{
		private StreamWriter writer = null;

		/// <summary>
		/// Gets the outgoing body of data
		/// </summary>
		/// <value>The outgoing body of data.</value>
		public Stream OutgoingBody { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Extensions.BasicWeblings.Client.HttpOutgoingInteraction"/> class
		/// using a data stream and the initiating interaction
		/// </summary>
		/// <param name="bodyStream">Data body stream.</param>
		/// <param name="parent">Parent.</param>
		public HttpOutgoingInteraction (Stream bodyStream, IInteraction parent) : base(parent)
		{
			this.OutgoingBody = bodyStream;
		}

		/// <summary>
		/// Gets a streamwriter for writing text to the outgoing body of data.
		/// </summary>
		/// <returns>The outgoing body writer.</returns>
		public StreamWriter GetOutgoingBodyWriter() {
			if (writer == null)
				writer = new StreamWriter (OutgoingBody);

			return writer;				                       
		}

		/// <summary>
		/// Gets a value indicating whether or not a writer has been produced for the stream
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public bool HasWriter() {
			return writer != null;
		}	

		/// <summary>
		/// Indicate all work with this instance is done; typically flushes data into underlying stream.
		/// </summary>
		public void Done ()
		{
			OutgoingBody.Flush();
		}

	}
}

