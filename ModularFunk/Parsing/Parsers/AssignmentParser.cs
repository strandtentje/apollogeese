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

			session.PushOffset();

			result = null;

			// dick
			if (identifierParser.Run (session, out identifier) > -1) {
				session.DeepenContext(identifier as String);
				// = 
				if (coupler.Run (session) > -1) {
					// "something";
					if (InnerParser.Run (session, out value) > 0) {
						session.ContextRegister(value);
						session.SurfaceContext(identifier as String);
						result = new Tuple<string, object> ((string)identifier, value);
						return 1;
					} else {
						session.PopOffset();
						return -1;
					}
				} else {
					session.PopOffset();
					return -1;
				}
			}

			session.PopOffset();
			return -1;
		}
	}
}