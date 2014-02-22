using System;
using BorrehSoft.Utensils.Parsing.Parsers;
using BorrehSoft.Utensils.Parsing;
using System.Collections.Generic;
using System.IO;

namespace BorrehSoft.Utensils.Collections.Settings
{
	public class IncludeParser : WhitespaceParser
	{
		private IdentifierParser identifierEater = new IdentifierParser();
		private StringParser valueEater = new StringParser ();
		private object dummy;
		private CharacterParser hashtagEater = new CharacterParser('#');

		public override string ToString ()
		{
			return string.Format ("include \"filename\"");
		}

		internal override int ParseMethod (ParsingSession session, out object result)
		{
			object identObj; string identifier;
			object valueObj; string value;

			result = null;

			//      #bla
			base.ParseMethod (session, out dummy); 									// consume whitespaces
			if (hashtagEater.ParseMethod (session, out dummy) > -1) {				// consume hashtag
				if ((identifierEater.ParseMethod (session, out identObj) > -1) && 	// consume and assign identifier
					(base.ParseMethod (session, out dummy) > 0) && 						// consume opener to include file
					(valueEater.ParseMethod (session, out valueObj) > 0)) 				// consume closer to include file
				{
					identifier = (string)identObj;
					value = (string)valueObj;

					if (identifier.ToLower () == "include") {
						if (File.Exists (value)) {
							string fileData = File.ReadAllText (value);

							session.Data = session.Data.Insert (
								session.Offset, 
								fileData);

							return fileData.Length;
						} else {
							throw new Exception ("During parsing, file not found: " + value);
						}
					} else {
						throw new ParsingException (session, identifierEater, "include");
					}
				}
				else {
					throw new ParsingException (session, this, "#");
				}
			}
			else {
				return 1;
			}
		}
	}
}

