using System;

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
			object unparsedIdentifier;

			result = null;

			if (base.ParseMethod (session, out unparsedIdentifier) > 0) {
				string identifier = unparsedIdentifier as String;

				if (session.References.Has (identifier)) {
					result = session.References [identifier];
					succescode = 1;
				} else {
					throw new ParsingException(session, this, identifier);
				}
			}

			return succescode;
		}
	}
}

