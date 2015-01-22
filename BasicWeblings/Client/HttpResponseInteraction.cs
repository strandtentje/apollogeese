using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.IO;

namespace BorrehSoft.Extensions.BasicWeblings.Client
{
	public class HttpResponseInteraction : QuickInteraction, IIncomingBodiedInteraction
	{		
		private StreamReader writer = null;

		public Stream IncomingBody { get; private set; }

		public StreamReader GetIncomingBodyReader() { 
			if (writer == null)
				writer = new StreamReader (IncomingBody);

			return writer;
		}

		public HttpResponseInteraction (Stream bodyStream, IInteraction parent) : base(parent)
		{
			IncomingBody = bodyStream;
		}

		public bool HasReader()
		{
			return writer != null;
		}
	}
}

