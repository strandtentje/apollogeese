using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Text;
using System.IO;

namespace Interactions
{
	public class Writer
	{
		public Encoding TargetEncoding { get; private set; }

		public Stream TargetStream { get; private set; }

		private Writer (Encoding encoding, Stream stream)
		{
			this.TargetEncoding = encoding;
			this.TargetStream = stream;
		}

		public static Writer Into (IOutgoingBodiedInteraction interaction)
		{
			return new Writer (interaction.Encoding, interaction.OutgoingBody);
		}

		public Writer Append (object value)
		{
			var encoded = TargetEncoding.GetBytes (value.ToString ());
			TargetStream.Write (encoded, 0, encoded.Length);
			return this;
		}
	}
}

