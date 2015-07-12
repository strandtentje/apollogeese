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

		protected virtual bool Couple(ParsingSession session, ref object identifier) {
			return coupler.Run (session) > -1;
		}

		internal override int ParseMethod (ParsingSession session, out object result)
		{
			object identifier, value;
			int returnValue = -1;

			result = null;

			// dick
			if (identifierParser.Run (session, out identifier) > -1) {
				// = 
				if (Couple(session, ref identifier)) {
					// "butt";
					session.DeepenContext(identifier as String);
					int iresult = InnerParser.Run (session, out value);
					if (iresult > 0) {
						session.ContextRegister(value);
						session.SurfaceContext(identifier as String);
						result = new Tuple<string, object> ((string)identifier, value);
						returnValue = 1;
					} else {
						session.SurfaceContext(identifier as String);
					}
				} 
			}

			return returnValue;
		}
	}
}