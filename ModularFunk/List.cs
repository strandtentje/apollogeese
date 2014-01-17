using System;
using System.Text;
using System.Text.RegularExpressions;

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
	}
}

