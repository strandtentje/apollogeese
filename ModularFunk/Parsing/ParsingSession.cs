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
		public Parser whitespaceParser;
		/// <summary>
		/// Gets the data to be parsed
		/// </summary>
		/// <value>
		/// The data.
		/// </value>
		public string Data { get; set; }

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
		/// Gets or sets the current column index.
		/// </summary>
		/// <value>The current column.</value>
		public int CurrentColumn { get; set; }

		public ParsingSession(string data, Parser whitespaceParser)
		{
			this.whitespaceParser = whitespaceParser;
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
			return FromFile (file, new WhitespaceParser ());
		}

		/// <summary>
		/// Reads all contents from a file for usage in a ParsingSession, and uses
		/// and alternative parser for whitespace regions (i.e. #operations)
		/// </summary>
		/// <returns>The file.</returns>
		/// <param name="file">File.</param>
		/// <param name="whitespaceParser">Whitespace parser.</param>
		public static ParsingSession FromFile(string file, Parser whitespaceParser)
		{
			return new ParsingSession (File.ReadAllText (file), whitespaceParser);
		}
	}
}

