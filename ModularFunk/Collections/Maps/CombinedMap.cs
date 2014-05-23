using System;
using BorrehSoft.Utensils.Collections;
using System.Collections.Generic;

namespace BorrehSoft.Utensils.Collections.Maps
{
	/// <summary>
	/// Map that is a combination of multiple maps. Instantiates quickly, seek times are
	/// Regular dictionary time * amount of underlying maps.
	/// </summary>
	public class CombinedMap<T> : Map<T>
	{
		List<Map<T>> fallBacks = new List<Map<T>>();
		Dictionary<string, byte> deletions = new Dictionary<string, byte>();

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Utensils.Collections.Maps.CombinedMap`1"/> class.
		/// </summary>
		/// <param name='maps'>
		/// Underling Maps. Will be evaluated in specified order.
		/// </param>
		public CombinedMap (params Map<T>[] maps) 
		{
			fallBacks.AddRange(maps);
		}

		/// <summary>
		/// Gets a combinatino of all underlying dictionaries. 
		/// </summary>
		/// <value>
		/// The combined dictionary. 
		/// </value>
		public override Dictionary<string, T> Dictionary {
			get {				
				foreach (Map<T> fallBack in fallBacks) {
					foreach(KeyValuePair<string, T> pair in fallBack.Dictionary)
					{
						if (!base.Has(pair.Key)) {
							if (!deletions.ContainsKey(pair.Key)) {
								base.Add(pair.Key, pair.Value);
							}
						}
					}
				}

				return base.Dictionary;
			}
		}

		/// <summary>
		/// Add the specified key and value.
		/// </summary>
		/// <param name='key'>
		/// Key.
		/// </param>
		/// <param name='value'>
		/// Value.
		/// </param>
		protected override void Add (string key, T value)
		{
			base.Add(key, value);
			if (deletions.ContainsKey(key)) deletions.Remove(key);
		}

		/// <summary>
		/// Delete the specified key.
		/// </summary>
		/// <param name='key'>
		/// Key.
		/// </param>
		protected override void Delete (string key)
		{
			if (base.Has(key)) base.Delete(key);

			foreach (Map<T> fallBack in fallBacks) {
				if (fallBack.Has (key)) {
					deletions.Add (key, 1);
					return;
				}
			}
		}

		/// <summary>
		///  Determines whether this instance has the specified key. 
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance has key; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='key'>
		/// If set to <c>true</c> key.
		/// </param>
		public override bool Has (string key)
		{
			if (base.Has(key)) return true;

			if (deletions.ContainsKey(key)) return false;

			foreach(Map<T> fallBack in fallBacks)
				if (fallBack.Has(key)) 
					return true;

			return false;
		}

		/// <summary>
		///  Get item at the specified key. 
		/// </summary>
		/// <param name='key'>
		///  Key. 
		/// </param>
		public override T Get (string key)
		{
			if (base.Has (key))
				return base [key];

			if (!deletions.ContainsKey (key)) {
				foreach (Map<T> fallBack in fallBacks)
					if (fallBack.Has (key)) 
						return fallBack.Get (key);
			}

			throw new KeyNotFoundException ("Key wasn't found in any of the underlying maps.");
		}
	}
}
