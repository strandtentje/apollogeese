using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using System.Text;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	/// <summary>
	/// Outgoing Interaction for HTTP communications
	/// </summary>
	public class SimpleOutgoingInteraction :  SimpleInteraction, IOutgoingBodiedInteraction
	{
		private StreamWriter writer = null;

		/// <summary>
		/// Gets the outgoing body of data
		/// </summary>
		/// <value>The outgoing body of data.</value>
		public Stream OutgoingBody { get; private set; }

		public SimpleOutgoingInteraction(Stream bodyStream, IInteraction parent) : base(parent) {
			this.OutgoingBody = bodyStream;
			this.Encoding = Encoding.UTF8;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Extensions.BasicWeblings.Client.HttpOutgoingInteraction"/> class
		/// using a data stream and the initiating interaction
		/// </summary>
		/// <param name="bodyStream">Data body stream.</param>
		/// <param name="parent">Parent.</param>
		public SimpleOutgoingInteraction (Stream bodyStream, Encoding encoding, IInteraction parent) : base(parent)
		{
			this.OutgoingBody = bodyStream;
			this.Encoding = encoding;
		}

		public Encoding Encoding { get; private set; }

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
		public virtual void Done ()
		{
			if (HasWriter ())
				writer.Flush ();
			OutgoingBody.Flush();
		}

	}
}

