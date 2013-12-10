using System;
using ModularFunk.Parsing;

namespace ModularFunk.Parsing.Parsers
{
	public class AssignmentParser : Parser
	{
		public override string ToString ()
		{
			return "Assignment";
		}

		private override int ParseMethod (ParsingSession session, out object result)
		{
			return 0;
		}
	}

}

