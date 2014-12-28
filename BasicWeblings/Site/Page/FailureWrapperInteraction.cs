using System;
using System.Text;
using BorrehSoft.ApolloGeese.Duckling;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.Duckling.Http;
using System.Web;
using BorrehSoft.Utensils.Collections;
using System.IO;

namespace BorrehSoft.Extensions.BasicWeblings
{
	/// <summary>
	/// Failure wrapper interaction.
	/// </summary>
	class FailureWrapperInteraction : QuickInteraction, IOutgoingBodiedInteraction
	{
		public FailureWrapperInteraction (IInteraction parameters) : base(parameters)
		{
			body = new MemoryStream ();
			outgoingBody = new StreamWriter (body);
		}

		private MemoryStream body;
		private StreamWriter outgoingBody;

		public StreamWriter OutgoingBody { 
			get {
				return outgoingBody;
			}
		}

		/// <summary>
		/// Gets the text and closes.
		/// </summary>
		/// <returns>The text.</returns>
		public string GetTextAndClose()
		{
			outgoingBody.Flush ();

			string data;
			body.Position = 0;

			using (StreamReader reader = new StreamReader (body)) {
				 data = reader.ReadToEnd ();
			}

			return data;
		}
	}
}

