using System;
using System.Text;
using System.Text.RegularExpressions;

namespace BorrehSoft.Utensils
{
	public class StringParser : Parser
	{
		public override string ToString ()
		{
			return "A quotation-mark enclosed, backslash-escaped string.";
		}

		Regex stringRegex = new Regex(@"""[^""\\]*(?:\\.[^""\\]*)*""");

		internal override int ParseMethod (ParsingSession session, out object result)
		{
			// "
			if (session.Data[session.Offset] == '\"') {
				string entirity, quotedText, escapedText, text;

				// "cheese"
				entirity = session.Data.Substring(session.Offset);

				Match quoteMatch = stringRegex.Match (entirity);

				quotedText = quoteMatch.Value;
				escapedText = quotedText.Remove(quotedText.Length - 1, 1).Remove(0, 1);
				text = Regex.Unescape(escapedText);

				session.Offset += quotedText.Length;

				result = text;
				return text.Length;
			}

			result = null;
			return -1;
		}
	}
}

