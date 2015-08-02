using System;
using System.Collections.Generic;

namespace BorrehSoft.Utensils.Parsing.Parsers
{
	public class ReferenceParser : IdentifierParser
	{
		public ReferenceParser() : base(ReferenceValidator)
		{
		}

		static bool ReferenceValidator(char ch)
		{
			return IsAlphaNumericUsc(ch) || (ch == '.');
		}

		public override string ToString ()
		{
			return "Valid Reference";
		}

		internal override int ParseMethod (ParsingSession session, out object result)
		{
			int succescode = 0;
			object unparsedIdentifier, dummy;
			CharacterParser leadingDot = new CharacterParser ('.');
			List<string> contextString = null;
			
			result = null;

			int bookmark = session.Offset;

			while (leadingDot.ParseMethod(session, out dummy) > 0) {
				if (contextString == null) { 
					contextString = new List<string>(session.Context.ToArray());
					contextString.Reverse();
				}

				contextString.RemoveAt(contextString.Count - 1);
				succescode--;
			}

			string identifier = (contextString == null ? "" : string.Join(".", contextString.ToArray()) + ".");

			if (base.ParseMethod (session, out unparsedIdentifier) > 0) {
				identifier += unparsedIdentifier as String;

				if (session.References.Has (identifier)) {
					result = session.References [identifier];
					succescode = 1;
				} else {
					session.Offset = bookmark;
					if (succescode < 0)
						throw new Exception ("Invalid Reference to " + identifier);
					else 
						succescode = -1;
				}
			}

			return succescode;
		}
	}
}
