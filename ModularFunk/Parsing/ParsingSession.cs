using System;
using System.IO;

namespace ModularFunk.Parsing
{
	public class ParsingSession
	{
		private object dummy;
		private int offset = 0;

		public string Data { get; private set; }
		public int CurrentLine { get; private set; }

		public ParsingSession(string data, int offset = 0)
		{
			this.Data = data;
			this.offset = offset;
			this.CurrentLine = 0;
		}

		public static ParsingSession FromFile(string file)
		{
			return new ParsingSession(File.ReadAllText(file));
		}

		public object Get (params Parser[] parsers)
		{
			WhitespaceParser.ParseWhitespace (Data, ref offset, out dummy);

			object result;

			foreach (Parser parser in parsers) {
				if (parser.Run(Data, ref offset, out result) > -1)
					return result;
			}

			throw new ParsingException(CurrentLine, parsers);
		}
	}
}

