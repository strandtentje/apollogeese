using System;
using System.Text;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Collections.Generic;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;
using System.Text.RegularExpressions;
using System.Web;
using BorrehSoft.Utilities.Collections;
using System.IO;

namespace BorrehSoft.ApolloGeese.Extensions.InputProcessing
{
	/// <summary>
	/// Failure wrapper interaction.
	/// </summary>
	class FailureWrapperInteraction : SimpleInteraction, IOutgoingBodiedInteraction
	{
		private MemoryStream body;
		private StreamWriter writer = null;
		public Encoding Encoding { get; private set; }

		public FailureWrapperInteraction (IInteraction parameters, Encoding encoding) : base(parameters)
		{
			body = new MemoryStream ();

			this.Encoding = encoding;
		}

		public Stream OutgoingBody {
			get {
				return body;
			}
		}

		public StreamWriter GetOutgoingBodyWriter() {
			if (writer == null)
				writer = new StreamWriter (body, Encoding);

			return writer;
		}

		public bool HasWriter() {
			return writer != null;
		}

		/// <summary>
		/// Gets the text and closes.
		/// </summary>
		/// <returns>The text.</returns>
		public string GetTextAndClose()
		{
			if (HasWriter ())
				GetOutgoingBodyWriter ().Flush ();

			string data;
			body.Position = 0;

			using (StreamReader reader = new StreamReader (body)) {
				 data = reader.ReadToEnd ();
			}

			return data;
		}
	}
}

