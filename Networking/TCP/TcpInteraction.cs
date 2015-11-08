using System;
using System.Net.Sockets;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using System.Text;

namespace BorrehSoft.ApolloGeese.Extensions.Networking
{
	public class TcpInteraction : SimpleInteraction, IIncomingBodiedInteraction, IOutgoingBodiedInteraction
	{
		private TcpClient Client;

		public string SourceName { get; private set; }

		public Stream IncomingBody { get; private set; }

		private StreamReader reader = null;

		public TextReader GetIncomingBodyReader() {
			if (reader == null) {
				reader = new StreamReader (IncomingBody);
			}

			return reader;
		}

		public Encoding Encoding {
			get {
				return Encoding.UTF8;
			}
		}

		public bool HasReader() {
			return reader != null;
		}

		public Stream OutgoingBody { get; private set; }

		private StreamWriter writer = null;

		public StreamWriter GetOutgoingBodyWriter() {
			if (writer == null) {
				writer = new StreamWriter (OutgoingBody);
			}

			return writer;
		}

		public bool HasWriter() {
			return writer != null;
		}

		public TcpInteraction (TcpClient client, long ticker = 0)
		{
			this.Client = client;
			this.SourceName = string.Format ("TcpClient{0}", ticker);
			this.IncomingBody = client.GetStream ();
			this.OutgoingBody = client.GetStream ();
		}
	}
}

