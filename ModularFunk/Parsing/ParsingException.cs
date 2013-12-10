using System;
using System.IO;
using ModularFunk.Parsing.Parsers;

namespace ModularFunk.Parsing
{
	class ParsingException : Exception
	{
		public ParsingException (int currentLine, Parser[] parsers) : 
			base (
				string.Format(
					"Expected {0} at line {1}", parsers.ToString(), currentLine.ToString()))
		{
		}
	}

}

