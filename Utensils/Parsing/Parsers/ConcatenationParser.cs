using System;
using BorrehSoft.Utensils.Collections;
using System.Collections.Generic;

namespace BorrehSoft.Utensils.Parsing.Parsers
{
	/// <summary>
	/// Concatenation parser.
	/// </summary>
	public class ConcatenationParser : Parser
	{
		public Parser InnerParser { get; set; }
		CharacterParser opener, closer, coupler;

		public override string ToString ()
		{
			return string.Format (
				"A concatenation coupled with {0}, opened by a {1} and closed by a {2}", 
				coupler.TargetCharacter, opener.TargetCharacter, closer.TargetCharacter);
		}

		public ConcatenationParser (char openerChar, char closerChar, char couplerChar)
		{
			this.opener = new CharacterParser (openerChar);
			this.closer = new CharacterParser (closerChar);
			this.coupler = new CharacterParser (couplerChar);
		}

		internal override int ParseMethod (ParsingSession session, out object result)
		{
			if (opener.Run (session) > 0) {
				List<object> target = new List<object>();
				object parsed;

				bool coupled = true;

				while (coupled && (InnerParser.Run(session, out parsed) > 0)) {
					target.Add (parsed);
					coupled = false;
					while (coupler.Run (session) > 0)
						coupled = true;
				}

				int closerResult = closer.Run (session);

				if (closerResult > 0) {
					result = target;
					return target.Count + 1;
				} else {
					string trail, ahead;
					trail = session.GetTrail();
					ahead = session.GetAhead();

					throw new ParsingException (session, closer, trail, ahead);
				}
			}

			result = null;
			return -1;
		}
	}
}

