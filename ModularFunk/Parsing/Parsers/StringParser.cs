using System;
using System.Text;
using System.Text.RegularExpressions;

namespace BorrehSoft.Utensils.Parsing.Parsers
{
	public class StringParser : Parser
	{
		public override string ToString ()
		{
			return "A quotation-mark enclosed, backslash-escaped string.";
		}

		/// <summary>
		/// Regex that distinguishes quotationmark-enclosed strings.
		/// I probably nicked this from a StackOverflow answer.
		/// TODO: Find the person, buy them a beer and give crebit where due.
		/// </summary>
		Regex stringRegex = new Regex(@"""[^""\\]*(?:\\.[^""\\]*)*""");

		internal override int ParseMethod (ParsingSession session, out object result)
		{
			// "
			if (session.Data[session.Offset] == '\"') {
				if (session.Data[session.Offset + 1] != '\"') {
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
				} else {
					session.Offset += 2;
					result = "";
					return 1;
				}
			}

			result = null;
			return -1;
		}
	}
}

