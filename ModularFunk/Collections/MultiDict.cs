using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BorrehSoft.Utensils
{
	/// <summary>
	/// Dict with multiple definitions for one key.
	/// </summary>
	public class MultiDict<TKey, TValue> : Dictionary<TKey, List<TValue>>
	{
		/// <summary>
		/// Appends a new value to a key.
		/// </summary>
		/// <param name='key'>
		/// Key.
		/// </param>
		/// <param name='value'>
		/// Value.
		/// </param>
		public void AppendTo(TKey key, TValue value) 
		{
			if (!this.ContainsKey(key))
				base.Add(key, new List<TValue>());

			this[key].Add(value);
		}

		/// <summary>
		/// Removes a value from from a key.
		/// </summary>
		/// <param name='key'>
		/// Key.
		/// </param>
		/// <param name='value'>
		/// Value.
		/// </param>
		public void RemoveFrom(TKey key, TValue value)
		{
			this[key].Remove(value);
		}

		public void FillWith (List<TKey> keys, TValue value)
		{
			foreach(TKey key in keys)
				this.AppendTo(key, value);
		}

	}
}

