using System;
using BorrehSoft.Utensils.Parsing;

namespace BorrehSoft.Utensils.Parsing.Parsers
{
	public class AssignmentParser : Parser
	{
		IdentifierParser identifierParser = new IdentifierParser();
		CharacterParser coupler;
		public Parser InnerParser { get; set; }

		public AssignmentParser(char couplerChar = '=')
		{
			coupler = new CharacterParser(couplerChar);
		}

		public override string ToString ()
		{
			return "Assignment";
		}

		internal override int ParseMethod (ParsingSession session, out object result)
		{
			object identifier, value;

			result = null;

			// dick
			if (identifierParser.Run (session, out identifier) > -1) {
				// = 
				if (coupler.Run (session) > -1) {
					// "butt";
					session.DeepenContext(identifier as String);
					if (InnerParser.Run (session, out value) > 0) {
						session.ContextRegister(value);
						session.SurfaceContext(identifier as String);
						result = new Tuple<string, object> ((string)identifier, value);
						return 1;
					} else {
						session.SurfaceContext(identifier as String);
					}
				} 
			}

			return -1;
		}
	}
}