using System;
using BorrehSoft.Utensils.Parsing;
using BorrehSoft.Utensils.Parsing.Parsers;
using System.Collections.Generic;

namespace BorrehSoft.Utensils.Settings
{
	/// <summary>
	/// Settings parser.
	/// </summary>
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
		IdentifierParser identifierParser = new IdentifierParser();
		StringParser stringParser = new StringParser();
		ValueParser<int> 	intParser = 	new ValueParser<int>(	int.TryParse	);
		ValueParser<float> 	floatParser = 	new ValueParser<float>(	float.TryParse	);
		ValueParser<bool> 	boolParser = 	new ValueParser<bool>(	bool.TryParse, 	"(True|False|true|false)");
		AssignmentParser assignmentParser;

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Utensils.Settings.SettingsParser"/> class.
		/// </summary>
		public SettingsParser()
		{
			assignmentParser = new AssignmentParser(
				intParser, 
				floatParser, 
				boolParser,
				identifierParser, 
				stringParser, 
				this
				);
		}

		/// <summary>
		/// Parsing Method for the <see cref="BorrehSoft.Utensils.Settings"/> type.
		/// </summary>
		/// <returns>
		/// Succes value; zero or higher when succesful.
		/// </returns>
		/// <param name='session'>
		/// Session in which this parsing action will be conducted.
		/// </param>
		/// <param name='result'>
		/// Result of this parsing action
		/// </param>
		internal override int ParseMethod (ParsingSession session, out object result)
		{
			if (blockOpener.Run (session) > 0) {
				Settings map = new Settings ();

				object parsed;

				while (assignmentParser.Run (session, out parsed) > 0)
				{
					Tuple<string, object> assignment = (Tuple<string, object>)parsed;
					map[assignment.Key] = assignment.Value;
					session.Get(lineCloser);
				}

				session.Get(blockCloser);

				result = map;

				return map.Count;
			}

			result = null;

			return -1;
		}	
	}
}

