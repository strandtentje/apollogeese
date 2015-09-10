using System;
using System.Text.RegularExpressions;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public abstract class KeywordService : Service
	{
		private string splitterRegex;

		[Instruction("Regex that is used to seperate keywords.", @"\W|_")]
		public string KeywordSplitterRegex { 
			get {
				return splitterRegex;
			}
			set {
				splitterRegex = value;
				KeywordSplitter = new Regex (splitterRegex);
			}
		}

		/// <summary>
		/// Gets or sets the keyword splitter used to turn a query string into seperate query words.
		/// </summary>
		/// <value>The keyword splitter.</value>
		public Regex KeywordSplitter { get; set; }
	}
}

