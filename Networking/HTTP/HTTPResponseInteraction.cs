using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Text;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Log;
using System.Net;
using System.IO;

namespace Networking
{
	class HTTPResponseInteraction : BareInteraction, IIncomingBodiedInteraction
	{
		public HTTPResponseInteraction (HttpWebRequest request, HttpWebResponse response, IInteraction parameters) : base(parameters)
		{
			this.SourceName = request.RequestUri.ToString ();
			this.IncomingBody = response.GetResponseStream ();
		}

		public string SourceName { get; private set; }

		public Stream IncomingBody { get; private set; }

		private TextReader reader = null;

		public bool HasReader() {
			return reader != null;
		}

		public TextReader GetIncomingBodyReader() {
			if (!HasReader ()) {
				reader = new StreamReader (this.IncomingBody);
			}

			return reader;
		}

		public override IInteraction Clone (IInteraction parent)
		{
			throw new UnclonableException ();
		}
	}

}

