using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.IO;

namespace BorrehSoft.Extensions.BasicWeblings.Client
{
	public class HttpResponseInteraction : QuickInteraction, IIncomingBodiedInteraction
	{		
		StreamReader bodyWriter; 

		public HttpResponseInteraction (Stream bodyStream, IInteraction parent) : base(parent)
		{
			bodyWriter = new StreamReader(bodyStream);
		}

		public StreamReader IncomingBody { get { return bodyWriter; } }
	}
}

