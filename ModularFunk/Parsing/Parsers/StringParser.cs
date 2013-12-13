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

		CharacterParser quotationMark = new CharacterParser('"');
		Regex stringRegex = new Regex(@"""[^""\\]*(?:\\.[^""\\]*)*""");

		internal override int ParseMethod (ParsingSession session, out object result)
		{
			if (quotationMark.Run (session) > 0) {
				string entirity, quotedText, escapedText, text;

				entirity = session.Data.Remove(session.Offset);
				quotedText = stringRegex.Match(entirity).Value;
				escapedText = quotedText.Remove(0, 1).Remove(quotedText.Length - 1, 1);
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

