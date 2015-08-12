using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections.Maps.Search;
using BorrehSoft.Utensils.Collections;
using System.Text.RegularExpressions;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Lookup
{
	public abstract class LookupMutator : Service
	{
		[Instruction("Name of context variable to use as lookup query.")]
		public string LookupKeyName { get; set; }

		[Instruction("Name of the lookup this instance writes to.")]
		public string LookupName {	get; set; }

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
		private Regex KeywordSplitter { get; set; }
	}

}

