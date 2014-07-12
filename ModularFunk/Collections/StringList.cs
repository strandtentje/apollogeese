using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Generic;

namespace BorrehSoft.Utensils.Collections
{
	/// <summary>
	/// List of string chunks, useful methods inside!
	/// </summary>
	public class StringList : Queue<string>
	{
		public delegate string DecoderMethod (string inString);		

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Utensils.StringList"/> class.
		/// </summary>
		public StringList() {	}

		public string Original {
			get;
			private set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Utensils.StringList"/> class,
		/// from a string concatenated with a certain sepeartor
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="seperator">Seperator.</param>
		public StringList (string data, char seperator, DecoderMethod decoder = null)
		{
			this.Original = data;
			this.Seperator = seperator;

			string[] dataChunks = data.Trim (seperator).Split (seperator);

			if (decoder == null) 
				foreach(string dataChunk in dataChunks)
					this.Enqueue(dataChunk);

			else 
				foreach (string dataChunk in dataChunks) 
					this.Enqueue (decoder (dataChunk));

		}

		/// <summary>
		/// Adds unique regex matches.
		/// </summary>
		/// <returns>The unique regex matches.</returns>
		/// <param name="blob">BLOB.</param>
		/// <param name="pattern">Pattern.</param>
		public MatchCollection AddUniqueRegexMatches(string blob, string pattern) 
		{
			MatchCollection replaceables = Regex.Matches (blob, pattern, RegexOptions.IgnoreCase);

			foreach (Match chunkMatch in replaceables) {
				string chunkName = chunkMatch.Groups [1].Value;
				if (!this.Contains (chunkName))
					this.Enqueue (chunkName);
				/* if ((modSettings [chunkName] != null) && 
					(!defaultVariables.ContainsKey (chunkName)))
					defaultVariables.Add (chunkName, (string)modSettings [chunkName]); */
			}

			return replaceables;
		}

		/// <summary>
		/// Gets the previous cursor.
		/// </summary>
		/// <value>
		/// The previous cursor.
		/// </value>
		public int PreviousCursor { get; private set; }

		/// <summary>
		/// Gets the cursor position
		/// </summary>
		/// <value>The cursor.</value>
		public int Cursor { get; private set; }

		/// <summary>
		/// Gets the seperator character passed during construction
		/// </summary>
		/// <value>
		/// The seperator character
		/// </value>
		public char Seperator { get; private set; }

		/// <summary>
		/// Reads to the end of the series
		/// </summary>
		/// <returns>
		/// Everything up to the end
		/// </returns>
		public string ReadToEnd ()
		{
			string combined = string.Join("/", this.ToArray());

			this.Clear();

			return combined;
		}
	}
}
