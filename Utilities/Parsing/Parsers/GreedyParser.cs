using System;
using System.Text;

namespace BorrehSoft.Utensils.Parsing.Parsers
{
	public class GreedyParser : Parser
	{
		CharacterParser ParseDelimiter { get; set; }

		public GreedyParser (char delimiter)
		{
			this.ParseDelimiter = new CharacterParser (delimiter);
		}

		internal override int ParseMethod (ParsingSession session, out object result)
		{
			StringBuilder resultBuilder = new StringBuilder ();

			while (this.ParseDelimiter.Run(session) < 0)
				resultBuilder.Append (session.Data [session.Offset++]);

			result = resultBuilder.ToString ();

			return resultBuilder.Length;
		}
	}
}

