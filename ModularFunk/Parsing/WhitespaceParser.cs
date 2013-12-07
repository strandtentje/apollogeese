using System;

namespace ModularFunk.Parsing
{
	public class WhitespaceParser : Parser
	{
		public override string ToString ()
		{
			return "Whitespace";
		}

		public override ParseMethod Run {
			get {
				return ParseWhitespace;
			}
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
		public static int ParseWhitespace(string data, ref int offset, out object result)
		{
			int newlineAmount = 0;

			bool currentCRLF, previousCRLF = false;

			for (char c = data[offset]; IsSpace(c); c = data[offset]) {
				currentCRLF = IsNewline(c);

				newlineAmount += (currentCRLF && !previousCRLF ? 1 : 0);

				previousCRLF = currentCRLF;
			}

			result = null;

			return newlineAmount;
		}
	}
}

