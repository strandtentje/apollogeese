using System;
using System.Text.RegularExpressions;

namespace BorrehSoft.Utensils.Parsing.Parsers
{
	public class WhitespaceParser : Parser
	{
		/// <summary>
		/// The white space and comment finding regex.
		/// What a beautiful abomination is it not?
		/// </summary>
		Regex whiteSpaceRegex = new Regex(@"(\/\*.*\*\/|\/\/.*\n| |\r|\n|\t)+");

		public override string ToString ()
		{
			return "Whitespace";
		}

		/// <summary>
		/// Tries to parse whitespace
		/// </summary>
		/// <returns>
		/// Amount of newlines
		/// </returns>
		/// <param name='data'>
		/// Data to parse
		/// </param>
		/// <param name='offset'>
		/// Offset in data
		/// </param>
		/// <param name='result'>
		/// Nothing
		/// </param>
		internal override int ParseMethod(ParsingSession session, out object result)
 		{
			Match oncomingWhitespace = whiteSpaceRegex.Match (
				session.Data.Substring (session.Offset));

			result = null;
			if (!oncomingWhitespace.Success)
				return -1;

			if (oncomingWhitespace.Index != 0)
				return -1;

			string consecSpace = oncomingWhitespace.Value;

			session.Offset += consecSpace.Length;

			int lastBreak = 0; 

			for (int charI = 0; charI < consecSpace.Length; charI++) {
				if (consecSpace [charI] == '\n') {
					lastBreak = charI;
					session.CurrentLine++;
				}
			}

			session.CurrentColumn = consecSpace.Length - lastBreak;

			result = oncomingWhitespace.Value;
			return oncomingWhitespace.Length;
		}
	}
}

