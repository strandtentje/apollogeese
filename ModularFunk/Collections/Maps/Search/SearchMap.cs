using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace BorrehSoft.Utensils.Collections.Maps.Search
{
	/// <summary>
	/// Search map.
	/// </summary>
	public class SearchMap<T> : Map<CleverSet<T>> where T : IIndexable
	{
		/// <summary>
		/// Add the specified item to this searchable map.
		/// </summary>
		/// <param name='item'>
		/// Item.
		/// </param>
		public void Add (T item)
		{
			CleverSet<T> targetSet;

			foreach (string keyword in item.Keywords) {
				if (base.Has(keyword))
				{
					targetSet = base[keyword];
				} else {
					targetSet = new CleverSet<T>();
					base[keyword] = targetSet;
				}

				if (!targetSet.ContainsKey(item.Meta))
					targetSet.Add (item.Meta, item);
			}
		}

		/// <summary>
		/// Find items in this SearchMap using the spcified search string
		/// </summary>
		/// <param name='query'>
		/// Search string. May contain boolean list modifiers such as OR and AND.
		/// </param>
		public CleverSet<T> Find (string[] query)
		{
			BooleanOperator 	currentOperator	= BooleanOperator.OR;
			Queue<string> 		queryChain		= new Queue<string> (query);
			CleverSet<T>		resultSet		= new CleverSet<T>();

			while (queryChain.Count > 0) {
				string shackle = queryChain.Dequeue();

				if (!Enum.TryParse(shackle, out currentOperator))
				{
					CleverSet<T> nextSet = Find (shackle);

					if (currentOperator == BooleanOperator.AND)	
						resultSet.PreserveOnlyFrom(nextSet);

					else if (currentOperator == BooleanOperator.OR)
						resultSet.AddFrom(nextSet);
				}
			}

			return resultSet;
		}

		/// <summary>
		/// Returns the same list as the other Find, but sorted.
		/// </summary>
		/// <returns>
		/// The sorted list
		/// </returns>
		/// <param name='query'>
		/// Query.
		/// </param>
		public SortedList<string, T> FindSorted (string[] query)
		{
			return Find(query).ToSortedList();
		}

		/// <summary>
		/// Find items for the specified keyword.
		/// </summary>
		/// <param name='keyword'>
		/// Keyword.
		/// </param>
		public CleverSet<T> Find(string keyword)
		{
			return base[keyword] ?? new CleverSet<T>();
		}
	}
}
