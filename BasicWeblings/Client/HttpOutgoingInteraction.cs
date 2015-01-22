using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.IO;

namespace BorrehSoft.Extensions.BasicWeblings.Client
{
	public class HttpOutgoingInteraction :  QuickInteraction, IOutgoingBodiedInteraction
	{
		private StreamWriter writer = null;

		public Stream OutgoingBody { get; private set; }

		public HttpOutgoingInteraction (Stream bodyStream, IInteraction parent) : base(parent)
		{
			this.OutgoingBody = bodyStream;
		}

		public StreamWriter GetOutgoingBodyWriter() {
			if (writer == null)
				writer = new StreamWriter (OutgoingBody);

			return writer;				                       
		}

		public bool HasWriter() {
			return writer != null;
		}	

		public void Done ()
		{
			OutgoingBody.Flush();
		}

	}
}

