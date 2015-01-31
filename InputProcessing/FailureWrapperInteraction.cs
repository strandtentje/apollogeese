using System;
using System.Text;
using BorrehSoft.ApolloGeese.Duckling;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.Http;
using System.Web;
using BorrehSoft.Utensils.Collections;
using System.IO;

namespace BorrehSoft.Extensions.InputProcessing
{
	/// <summary>
	/// Failure wrapper interaction.
	/// </summary>
	class FailureWrapperInteraction : QuickInteraction, IOutgoingBodiedInteraction
	{
		private MemoryStream body;
		private StreamWriter writer = null;

		public FailureWrapperInteraction (IInteraction parameters) : base(parameters)
		{
			body = new MemoryStream ();
		}

		public Stream OutgoingBody {
			get {
				return body;
			}
		}

		public StreamWriter GetOutgoingBodyWriter() {
			if (writer == null)
				writer = new StreamWriter (body);

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

