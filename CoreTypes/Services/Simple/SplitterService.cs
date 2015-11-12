using System;
using System.Text.RegularExpressions;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public abstract class SplitterService : Service
	{
		private string splitterRegex;

		public const string WordSplitter = @"\W|_";

		[Instruction("Regex that is used to seperate keywords.", @"\W|_")]
		public string SplitterRegex { 
			get {
				return splitterRegex;
			}
			set {
				splitterRegex = value;
				Splitter = new Regex (splitterRegex);
			}
		}

		/// <summary>
		/// Gets or sets the keyword splitter used to turn a query string into seperate query words.
		/// </summary>
		/// <value>The keyword splitter.</value>
		public Regex Splitter { get; set; }
	}
}

