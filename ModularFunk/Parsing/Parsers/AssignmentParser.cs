using System;
using BorrehSoft.Utensils.Parsing;

namespace BorrehSoft.Utensils.Parsing.Parsers
{
	public class AssignmentParser : Parser
	{
		IdentifierParser identifierParser = new IdentifierParser();
		CharacterParser coupler = new CharacterParser('=');
		Parser[] expressionParsers;

		public AssignmentParser (params Parser[] exprParsers)
		{
			expressionParsers = exprParsers;
		}

		public override string ToString ()
		{
			return "Assignment";
		}

		internal override int ParseMethod (ParsingSession session, out object result)
		{
			object identifier, value;

			if (identifierParser.Run (session, out identifier) > -1) {
				if (coupler.Run(session) > -1) {
					value = session.Get(expressionParsers);

					result = new Tuple<string, object>((string)identifier, value);

					return 1;
				}
			}

			result = null;
			return -1;
		}
	}
}