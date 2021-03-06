using System;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using System.Net;
using System.IO;
using BorrehSoft.Utilities.Collections;
using Stringtionary = System.Collections.Generic.Dictionary<string, string>;
using System.Text;
using BorrehSoft.Utilities.Log;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	class Literal : Expression
	{
		public string PlainText;

		public Literal (string text)
		{
			this.PlainText = text;
		}

		public override bool TryWriteTo (StreamWriter writer, IInteraction context) {
			writer.Write (this.PlainText);
			writer.Flush ();
			return true;
		}
	}
}

