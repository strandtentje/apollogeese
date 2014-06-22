using System;
using System.IO;
using BorrehSoft.Utensils.Parsing.Parsers;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections;

namespace BorrehSoft.Utensils.Parsing
{
	/// <summary>
	/// Parsing session for one file. This component maintains the 
	/// serial data that will be parsed, the cursus position and current
	/// Line.
	/// </summary>
	public class ParsingSession
	{
		private Stack<string> context = new List<string>();

		private Stack<int> offsets = new Stack<int>();

		public Parser whitespaceParser;
		/// <summary>
		/// Gets the data to be parsed
		/// </summary>
		/// <value>
		/// The data.
		/// </value>
		public string Data { get; set; }

		/// <summary>
		/// Context Stack Name, i.e., if we're currently processing the item Alittle in Had of Mary, this will say
		/// Mary.Had.Alittle
		/// </summary>
		/// <value>
		/// The name of the context.
		/// </value>
		public string ContextName {
			get {
				return string.Join(".", context.ToArray());
			}
		}

		/// <summary>
		/// Gets a list representing context names from parentmost to childmost.
		/// </summary>
		/// <value>
		/// The context.
		/// </value>
		public Stack<string> Context {
			get { return context; }
			private set { 
				this.context = value;
			}
		}

		public Map<object> References { get; private set; }

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
			this.References = new Map<object>();
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

		/// <summary>
		/// Deepens the context with the supplied identifier.
		/// </summary>
		/// <param name='identifier'>
		/// Identifier.
		/// </param>
		public void DeepenContext (string identifier)
		{
			Context.Push(identifier);
		}

		public void ContextRegister(object reference)
		{
			References[ContextName] = reference;
		}

		/// <summary>
		/// Brings the context back to the surface by one.
		/// </summary>
		/// <param name='identifier'>
		/// Identifier.
		/// </param>
		public void SurfaceContext (string identifier)
		{
			if (Context.Peek() == identifier) {
				Context.Pop();
			} else {
				throw new Exception(string.Format(
					"A little accident occured where a deepened context wasn't surfaced properly. Line/Offset/Col/Context/Attempt: {0}/{1}/{2}/{3}/{4}",
					this.CurrentLine, this.Offset, this.CurrentColumn, this.ContextName, identifier));
			}
		}

		public void PushOffset()
		{
			this.offsets.Push(this.Offset);
		}

		public void PopOffset()
		{
			this.Offset = this.offsets.Pop();
		}
	}
}

