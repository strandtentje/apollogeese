using System;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections.Maps.Search;

namespace BorrehSoft.Utensils.Collections
{
	/// <summary>
	/// Hash set.
	/// </summary>
	public class CleverSet<T> : Dictionary<string, T> where T : IIndexable
	{
		public CleverSet ()
		{
		}

		public void Add (T item)
		{
			this.Add(item.Meta, item);
		}

		/// <summary>
		/// Produces a sorted list from this hashed list.
		/// </summary>
		/// <returns>
		/// The sorted list.
		/// </returns>
		public SortedList<string, T> ToSortedList ()
		{
			return new SortedList<string, T>(this);
		}		

		/// <summary>
		/// Preserves only items also in the specified set.
		/// </summary>
		/// <param name='nextSet'>
		/// The list with the items which need to be preserved
		/// </param>
		public void PreserveOnlyFrom (CleverSet<T> nextSet)
		{
			IEnumerable<string> keyCache = new List<string>(base.Keys);
				
			foreach(string key in keyCache)
				if (!nextSet.ContainsKey(key) && base.ContainsKey(key))
					base.Remove(key);
		}

		/// <summary>
		/// Adds from the specified set.
		/// </summary>
		/// <param name='nextSet'>
		/// The list from which items are going to be added to this set.
		/// </param>
		public void AddFrom (CleverSet<T> nextSet)
		{
			foreach (string key in nextSet.Keys) {
				if (!base.ContainsKey (key)) {
					IIndexable indexable = nextSet [key];
					if (indexable.Exists) {
						base.Add (key, (T)indexable);
					}
				}
			}
		}
	}
}

