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
		private AnyParser valueEater = new AnyParser (new FilenameParser(), new StringParser());
		private object dummy;
		private CharacterParser hashtagEater = new CharacterParser('#');

		public override string ToString ()
		{
			return string.Format ("include \"filename\"");
		}

		/// <summary>
		/// Includes a file into a session.
		/// </summary>
		/// <returns>
		/// The included file length
		/// </returns>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='session'>
		/// Session.
		/// </param>
		int IncludeFileIntoSession (string fileName, ParsingSession session)
		{
			if (File.Exists (fileName)) {
				Directory.SetCurrentDirectory((new FileInfo(fileName)).Directory.FullName); 

				string fileData = File.ReadAllText (fileName);

				session.Data = session.Data.Insert (
					session.Offset, 
					fileData);

				return fileData.Length;
			} else {
				throw new Exception ("During parsing, file not found: " + fileName);
			}
		}

		internal override int ParseMethod (ParsingSession session, out object result)
		{
			int resultCount;
			object identObj;
			string identifier;
			object valueObj;
			string value;

			result = null;

			resultCount = base.ParseMethod (session, out dummy); 

			if (hashtagEater.ParseMethod (session, out dummy) > 0) {
				if (identifierEater.ParseMethod (session, out identObj) < 0)
					throw new ParsingException (session, identifierEater, session.Trail);

				if (base.ParseMethod (session, out dummy) < 1)
					throw new ParsingException (session, this, session.Trail);

				if (valueEater.ParseMethod (session, out valueObj) < 0)
					throw new ParsingException (session, valueEater, session.Trail);

				identifier = (string)identObj;
				value = (string)valueObj;

				if (identifier.ToLower () == "include") 
					resultCount = IncludeFileIntoSession (value, session);
			}

			return resultCount;
		}
	}
}

