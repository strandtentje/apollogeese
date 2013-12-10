using System;
using ModularFunk.Parsing;
using ModularFunk.Parsing.Parsers;
using System.Collections.Generic;

namespace ModularFunk.Settings
{
	public class SettingsParser : Parser
	{
		public override string ToString ()
		{
			return "Settings, accolade-enclosed block with zero or more assignments";
		}

		CharacterParser 
			blockOpener = new CharacterParser('{'),
			blockCloser = new CharacterParser('}'),
			lineCloser = new CharacterParser(';');
		IdentifierParser identifierParser = new IdentifierParser(this);
		StringParser stringParser = new StringParser(this);
		ValueParser<int> intParser = new ValueParser<int>(this, int.TryParse);
		ValueParser<float> floatParser = new ValueParser<float>(this, float.TryParse);

		AssignmentParser assignmentParser = new AssignmentParser(
			intParser, floatParser, identifierParser, stringParser, this);

		public override int Run (ParsingSession session, out object result)
		{
			if (blockOpener.Run (session) > 0) {
				Settings map = new Settings ();

				Tuple<string, object> assignment;
				while (assignmentParser.Run (session, out assignment) > 0)
				{
					map[assignment.Key] = assignment.Value;
					session.Get(lineCloser);
				}

				session.Get(blockCloser);

				result = map;

				return map.Count;
			}

			return -1;
		}	
	}
}

