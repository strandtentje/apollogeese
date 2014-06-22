using System;

namespace BorrehSoft.Utensils.Parsing.Parsers
{
	public class ReferenceParser : IdentifierParser
	{
		internal override int ParseMethod (ParsingSession session, out object result)
		{
			object unparsedIdentifier;

			base.ParseMethod (session, out unparsedIdentifier);

			string identifier = unparsedIdentifier as String;

			if (identifier == null) {
				throw new NullReferenceException (
					"Tried parsing an identifier which turned out not to be textual." +
					"How did you manage that?"
				);
			}

			if (session.References.Has (identifier)) {
				result = session.References[identifier];
			}

			throw new NullReferenceException(string.Format("{0} refers to nothing at all (yet?)", identifier));
		}
	}
}

