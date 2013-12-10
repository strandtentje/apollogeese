using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ModularFunk.Parsing.Parsers
{
	public class StringParser : Parser
	{
		CharacterParser quotationMark = new CharacterParser('"');
		Regex stringRegex = new Regex(@"""[^""\\]*(?:\\.[^""\\]*)*""");

		private override int ParseMethod (ParsingSession session, out object result)
		{
			if (quotationMark.Run (session) > 0) {
				string entirity, quotedText, escapedText, text;
				StringBuilder returnValue = new StringBuilder();

				entirity = session.Data.Remove(session.Offset + 1);
				quotedText = stringRegex.Match(entirity).Value;
				escapedText = quotedText.Remove(0, 1).Remove(quotedText.Length - 1, 1);
				text = Regex.Unescape(escapedText);

				session.Offset += quotedText.Length;

				result = text;
				return text.Length;
			}

			return -1;
		}
	}
}

