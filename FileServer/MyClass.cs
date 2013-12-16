using System;
using BorrehSoft.ApolloGeese.Duckling;

namespace BorrehSoft.Extensions.FileServer
{
	public class FileProvider : ServiceProvider
	{
		public override bool Detect (System.Net.HttpListenerRequest request)
		{
			throw new NotImplementedException ();
		}

		public override ServiceParams Parse (System.Net.HttpListenerRequest request)
		{
			throw new NotImplementedException ();
		}
	}
}

