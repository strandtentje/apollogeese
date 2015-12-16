using System;
using System.Text;
using System.IO;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public class StringComposeInteraction : SimpleOutgoingInteraction
	{
		public StringComposeInteraction (IInteraction parent, Encoding encoding) : base(new MemoryStream(), encoding, parent)
		{

		}

		public override string ToString ()
		{
			if (HasWriter ()) {
				GetOutgoingBodyWriter ().Flush ();
			}

			this.OutgoingBody.Position = 0;

			using (StreamReader reader = new StreamReader (this.OutgoingBody, this.Encoding)) {
				return reader.ReadToEnd ();
			}
		}
	}
}

