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
	}

}

