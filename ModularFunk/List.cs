using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace BorrehSoft.Utensils
{
	public class List<T> : System.Collections.Generic.List<T>
	{
		public List()
		{

		}

		public List(System.Collections.Generic.IEnumerable<T> newContents) : base(newContents)
		{

		}

		public string[] ToStringArray ()
		{
			List<string> sarray = new List<string> ();

			foreach (T item in this)
				sarray.Add (item.ToString ());

			return sarray.ToArray ();
		}
	}

	public class StringList : List<string>
	{
		public delegate string DecoderMethod (string inString);		

		public StringList() {	}

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Utensils.StringList"/> class,
		/// from a string concatenated with a certain sepeartor
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="seperator">Seperator.</param>
		public StringList (string data, char seperator, DecoderMethod decoder = null)
		{
			string[] dataChunks =  data.Trim (seperator).Split (seperator);

			if (decoder == null) {
				this.AddRange (dataChunks);
			} else {
				foreach (string dataChunk in dataChunks) {
					this.Add (HttpUtility.UrlDecode (dataChunk));
				}
			}
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
					this.Add (chunkName);
				/* if ((modSettings [chunkName] != null) && 
					(!defaultVariables.ContainsKey (chunkName)))
					defaultVariables.Add (chunkName, (string)modSettings [chunkName]); */
			}

			return replaceables;
		}

		/// <summary>
		/// Gets the cursor position
		/// </summary>
		/// <value>The cursor.</value>
		public int Cursor { get; private set; }

		/// <summary>
		/// Reads a chunk and shifts the /-cursor once.
		/// </summary>
		/// <returns>The URL chunk.</returns>
		public string ReadUrlChunk()
		{
			return this [Cursor++];
		}

		/// <summary>
		/// Rewinds the cursor back one position.
		/// </summary>
		public void RewindUrl()
		{
			Cursor--;
		}

		/// <summary>
		/// Checks if the cursor is currently at the end of the string
		/// </summary>
		/// <returns><c>true</c>, if cursor is at end, <c>false</c> otherwise.</returns>
		public bool EndOfSeries
		{
			get {
				return Cursor >= Count;
			}
		}
	}
}

