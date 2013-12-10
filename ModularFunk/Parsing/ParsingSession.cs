using System;
using System.IO;
using ModularFunk.Parsing.Parsers;

namespace ModularFunk.Parsing
{
	public class ParsingSession
	{
		public WhitespaceParser whitespaceParser = 
			new WhitespaceParser(this);
		public string Data { get; private set; }
		public int CurrentLine { get; set; }
		public int Offset { get; private set; }

		public ParsingSession(string data)
		{
			this.Data = data;
			this.Offset = 0;
			this.CurrentLine = 0;
		}

		public static ParsingSession FromFile(string file)
		{
			return new ParsingSession(File.ReadAllText(file));
		}

		public object Get (params Parser[] parsers)
		{
			object value;

			foreach (Parser parser in parsers) {
				if (parser.Run(this, out value) > -1)
					return value;
			}

			throw new ParsingException(CurrentLine, parsers);
		}
	}
}

