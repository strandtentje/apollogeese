using System;
using System.Collections.Generic;

namespace BorrehSoft.Utensils.Collections.Maps.Search
{
	/// <summary>
	/// Interface for items which may be indexed into a search map
	/// </summary>
	public interface IIndexable : IComparable
	{
		/// <summary>
		/// Gets the keywords this item associates with
		/// </summary>
		/// <value>
		/// The keywords.
		/// </value>
		IEnumerable<string> Keywords { get; }

		string Meta { get; }
	}
}

