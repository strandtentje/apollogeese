using System;
using BorrehSoft.Utensils.Parsing;

namespace BorrehSoft.Utensils.Parsing.Parsers
{
	public class AssignmentParser : Parser
	{
		IdentifierParser identifierParser = new IdentifierParser();
		CharacterParser coupler = new CharacterParser('=');
		public Parser InnerParser { get; set; }

		public override string ToString ()
		{
			return "Assignment";
		}

		internal override int ParseMethod (ParsingSession session, out object result)
		{
			object identifier, value;

			// dick
			if (identifierParser.Run (session, out identifier) > -1) {
				// = 
				if (coupler.Run (session) > -1) {
					if (InnerParser.Run (session, out value) > 0) {
						result = new Tuple<string, object> ((string)identifier, value);
						return 1;
					} else {
						throw new ParsingException (session, InnerParser);
					}
				} else {
					throw new ParsingException (session, coupler, (string)identifier);
				}
			}

			result = null;
			return -1;
		}
	}
}