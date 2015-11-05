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
	public abstract class LookupAccessor : KeywordService
	{
		[Instruction("Name of context variable to use as lookup query.")]
		public string LookupKeyName { get; set; }

		[Instruction("Name of the lookup this instance writes to.")]
		public string LookupName {	get; set; }

		public override void LoadDefaultParameters (string defaultParameter)
		{
			string[] split = defaultParameter.Split ('[');

			if (split.Length > 1) {
				Settings ["lookupname"] = split [0];
				Settings ["lookupkeyname"] = split [1].TrimEnd (']');
			} else {
				throw new ArgumentException (
					"Default argument should be formatted like lookupname[indexvariable]");
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			LookupKeyName = modSettings.GetString ("lookupkeyname");
			LookupName = modSettings.GetString ("lookupname");
			KeywordSplitterRegex = modSettings.GetString("keywordsplitregex", @"\W|_");
		}
	}

}

