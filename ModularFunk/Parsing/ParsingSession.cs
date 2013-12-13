using System;
using System.IO;
using BorrehSoft.Utensils.Parsing.Parsers;

namespace BorrehSoft.Utensils.Parsing
{
	/// <summary>
	/// Parsing session for one file. This component maintains the 
	/// serial data that will be parsed, the cursus position and current
	/// Line.
	/// </summary>
	public class ParsingSession
	{
		public WhitespaceParser whitespaceParser = new WhitespaceParser();
		/// <summary>
		/// Gets the data to be parsed
		/// </summary>
		/// <value>
		/// The data.
		/// </value>
		public string Data { get; private set; }
		/// <summary>
		/// Gets or sets the current line index, readout intended
		/// for reporting parsing errors. Write intended for Parser-
		/// components
		/// </summary>
		/// <value>
		/// The current line.
		/// </value>
		public int CurrentLine { get; set; }
		/// <summary>
		/// Gets or sets the current cursor position within the
		/// serial data.
		/// </summary>
		/// <value>
		/// The offset.
		/// </value>
		public int Offset { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Utensils.Parsing.ParsingSession"/> class.
		/// </summary>
		/// <param name='data'>
		/// Data this Session will provide to the parsers.
		/// </param>
		public ParsingSession(string data)
		{
			this.Data = data;
			this.Offset = 0;
			this.CurrentLine = 0;
		}

		/// <summary>
		/// Reads all contents from a file for usage in a ParsingSession
		/// </summary>
		/// <returns>
		/// The immediately usable <see cref="BorrehSoft.Utensils.Parsing.ParsingSession"/>
		/// </returns>
		/// <param name='file'>
		/// Filename to read serial data from.
		/// </param>
		public static ParsingSession FromFile(string file)
		{
			return new ParsingSession(File.ReadAllText(file));
		}

		/// <summary>
		/// Get a chunk of data using any of the supplied parsers and errors out
		/// when no parser can produce an object from the current cursor position
		/// onwards.
		/// </summary>
		/// <param name='parsers'>
		/// Parsers to use.
		/// </param>
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

