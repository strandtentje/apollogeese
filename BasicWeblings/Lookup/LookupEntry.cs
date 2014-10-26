using System;
using BorrehSoft.Utensils.Collections.Maps.Search;
using BorrehSoft.ApolloGeese.Duckling;
using System.Text;
using System.Collections.Generic;

namespace BorrehSoft.Extensions.BasicWeblings.Lookup
{
	public class LookupEntry : IIndexable
	{
		IEnumerable<string> keywords;
		string meta;

		public LookupEntry (IEnumerable<string> keywords, IInteraction Parameters)
		{
			this.keywords = keywords;
			this.meta = Convert.ToBase64String((new Guid()).ToByteArray());
			this.Parameters = Parameters;
		}

		public IInteraction Parameters {
			get; private set; 
		}

		public IEnumerable<string> Keywords { 
			get {
				return this.keywords;
			}
		}

		public string Meta {
			get {
				return this.meta;
			}
		}

		public int CompareTo(object other)
		{
			LookupEntry otherEntry = other as LookupEntry;

			if (otherEntry == null) return -1;

			return this.meta.CompareTo(otherEntry.meta);
		}
	}
}

