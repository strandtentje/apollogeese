using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.IO;

namespace BorrehSoft.Extensions.BasicWeblings.Client
{
	public class HttpOutgoingInteraction :  QuickInteraction, IOutgoingBodiedInteraction
	{
		private StreamWriter bodyWriter;

		public HttpOutgoingInteraction (Stream bodyStream)
		{
			bodyWriter = new StreamWriter(bodyStream);
		}

		public StreamWriter OutgoingBody { get { return bodyWriter; } }
	}
}

