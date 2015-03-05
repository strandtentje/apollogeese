using System;
using BorrehSoft.Utensils.Collections.Maps.Search;
using BorrehSoft.ApolloGeese.Duckling;
using System.Text;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Lookup
{
	/// <summary>
	/// Entry in hashed lookup
	/// </summary>
	public class LookupEntry : IIndexable
	{
		IEnumerable<string> keywords;
		string meta;

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Extensions.BasicWeblings.Lookup.LookupEntry"/> class using
		/// some keywords and a unique meta-identifier (when sourced from database, id is fine here.)
		/// </summary>
		/// <param name="keywords">Keywords this entry may be found at</param>
		/// <param name="meta">Metadata that may be used to check if this element equals another</param>
		/// <param name="Parameters">Upstream parameters that will be passed on for this entry.</param></param>
		public LookupEntry (IEnumerable<string> keywords, string meta, IInteraction Parameters)
		{
			this.keywords = keywords;
			this.meta = meta;
			this.Parameters = Parameters;
		}

		/// <summary>
		/// Gets the parameters.
		/// </summary>
		/// <value>The parameters.</value>
		public IInteraction Parameters {
			get; private set; 
		}

		/// <summary>
		/// Gets the keywords this item associates with
		/// </summary>
		/// <value>The keywords.</value>
		public IEnumerable<string> Keywords { 
			get {
				return this.keywords;
			}
		}

		/// <summary>
		/// Gets the meta-identifier used to check equality to other entries.
		/// </summary>
		/// <value>The meta-identifier</value>
		public string Meta {
			get {
				return this.meta;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="BorrehSoft.Extensions.BasicWeblings.Lookup.LookupEntry"/>
		/// (still) exists - if this is set to false, this entry may expect deletion.
		/// </summary>
		/// <value><c>true</c> if exists; otherwise, <c>false</c>.</value>
		public bool Exists { get; set; }

		/// <summary>
		/// Compares this entry to another; used for equality checking and sorting.
		/// </summary>
		/// <returns>The to.</returns>
		/// <param name="other">Other.</param>
		public int CompareTo(object other)
		{
			LookupEntry otherEntry = other as LookupEntry;

			if (otherEntry == null) return -1;

			return this.meta.CompareTo(otherEntry.meta);
		}
	}
}

