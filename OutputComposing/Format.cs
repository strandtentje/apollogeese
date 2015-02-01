using System;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	/// <summary>
	/// Formatting rule
	/// </summary>
	public class Format
	{
		Regex regex;
		string format;
		bool htmlentityescape;

		public Format(bool htmlentityescape) {
			this.htmlentityescape = htmlentityescape;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OutputComposing.Format"/> class.
		/// </summary>
		/// <param name="pattern">Pattern to look for</param>
		/// <param name="format">Format to put this pattern into</param>
		public Format (string pattern, string format)
		{
			this.regex = new Regex (pattern);
			this.format = format;
		}

		/// <summary>
		/// Apply format to input
		/// </summary>
		/// <param name="input">Input.</param>
		public string Apply(string input) {
			if (htmlentityescape)
				return HttpUtility.HtmlEncode (input);
			else			
				return regex.Replace (input, format);
		}
	}
}

