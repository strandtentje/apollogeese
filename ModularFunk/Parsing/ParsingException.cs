using System;
using System.IO;

namespace ModularFunk.Parsing
{
	class ParsingException : Exception
	{
		public ParsingException (int currentLine, Parser[] parsers) : 
			base (
				string.Format(
					"Expected {0} at line {1}", 
					string.Join(" or ", parsers), 
					currentLine.ToString()))
		{
		}
	}

}

