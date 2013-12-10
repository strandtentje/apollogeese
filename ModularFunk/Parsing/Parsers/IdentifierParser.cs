using System;
using System.Text;

namespace ModularFunk.Parsing.Parsers
{
	public class IdentifierParser : Parser
	{
		public override string ToString ()
		{
			return "Identifier";
		}

		/// <summary>
		/// Tries to parse an Identifier
		/// </summary>
		/// <returns>
		/// The parsed result
		/// </returns>
		private override int ParseMethod (ParsingSession session, out object result)
		{
			int position = session.Offset;

			StringBuilder resultBuilder = new StringBuilder ();

			result = "";

			if (IsAlpha (session.Data [position]))
				resultBuilder.Append (session.Data [position++]);
			else 
				return -1;

			for (session.Offset = position;
				(session.Offset < session.Data.Length) && IsAlphaNumeric(session.Data[position]);
				session.Offset++) {
				resultBuilder.Append (session.Data [position]);
			}

			result = resultBuilder.ToString();
			return resultBuilder.Length;
		}			
	}
}

