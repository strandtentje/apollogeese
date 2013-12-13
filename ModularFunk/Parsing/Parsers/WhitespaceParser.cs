using System;

namespace BorrehSoft.Utensils.Parsing.Parsers
{
	public class WhitespaceParser : Parser
	{
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
			int 
				newlineAmount = 0,
				whiteCount = 0;

			bool currentCRLF, previousCRLF = false;

			for (char c = session.Data[session.Offset]; IsSpace(c); c = session.Data[session.Offset]) {
				whiteCount++;

				currentCRLF = IsNewline(c);

				newlineAmount += (currentCRLF && !previousCRLF ? 1 : 0);

				previousCRLF = currentCRLF;
			}

			result = null;
			session.CurrentLine += newlineAmount;

			return -1;
		}
	}
}

