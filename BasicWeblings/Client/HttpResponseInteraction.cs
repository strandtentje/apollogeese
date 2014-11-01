using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.IO;

namespace BorrehSoft.Extensions.BasicWeblings.Client
{
	public class HttpResponseInteraction : QuickInteraction, IIncomingBodiedInteraction
	{		
		StreamReader bodyWriter; 

		public HttpResponseInteraction (Stream bodyStream)
		{
			bodyWriter = new StreamReader(bodyStream);
		}

		public StreamReader IncomingBody { get { return bodyWriter; } }
	}
}

