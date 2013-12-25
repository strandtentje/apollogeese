using System;
using System.IO;
using BorrehSoft.Utensils.Parsing.Parsers;

namespace BorrehSoft.Utensils.Parsing
{
	/// <summary>
	/// Parsing exception. This occurs when no suitable parser was found
	/// for the data at the current cursor position.
	/// </summary>
	class ParsingException : Exception
	{
		public ParsingException (ParsingSession session, Parser parser, string after = "") : 
			base (
				string.Format(
					"Expected {0} at line {1}, offset {2}, col {3}, after {4}", 
					parser.ToString(), 
					session.CurrentLine.ToString(), 
					session.Offset.ToString(),
					session.CurrentColumn.ToString(),
					after))
		{
		}
	}

}

