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
		public ParsingException (int currentLine, Parser parser) : 
			base (
				string.Format(
					"Expected {0} at line {1}", parser.ToString(), currentLine.ToString()))
		{
		}
	}

}

