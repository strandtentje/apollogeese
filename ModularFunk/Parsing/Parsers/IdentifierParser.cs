using System;
using System.Text;

namespace BorrehSoft.Utensils.Parsing.Parsers
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
		internal override int ParseMethod (ParsingSession session, out object result)
		{
			StringBuilder resultBuilder = new StringBuilder ();

			result = "";

			// d
			if (IsAlpha (session.Data [session.Offset]))
				resultBuilder.Append (session.Data [session.Offset++]);
			else 
				return -1;

			while(IsAlphaNumeric(session.Data[session.Offset]))
				resultBuilder.Append (session.Data [session.Offset++]);

			result = resultBuilder.ToString();
			return resultBuilder.Length;
		}			
	}
}

