using System;
using System.Collections.Generic;

namespace BorrehSoft.Utensils
{
	/// <summary>
	/// Easy to use map using the this[] property.
	/// </summary>
	public class Map<T>
	{
		/// <summary>
		/// The arse.
		/// </summary>
		private Dictionary<string, T> arse = new Dictionary<string, T> ();

		/// <summary>
		/// Gets the names.
		/// </summary>
		/// <returns>The names.</returns>
		public IEnumerable<string> GetNames() {
			return arse.Keys;
		}

		/// <summary>
		/// Gets the amount of mappings
		/// </summary>
		/// <value>The length.</value>
		public int Length { get { return arse.Count; } }

		/// <summary>
		/// Gets or sets the item with the specified name.
		/// </summary>
		/// <param name="name">Name</param>
		public T this[string name]
		{
			get	{
				if (arse.ContainsKey (name))
					return arse [name];
				return default(T);
			}
			set {
				if (arse.ContainsKey (name))
					arse.Remove (name);
				arse.Add (name, value);
			}
		}

		public Map<T> Clone ()
		{
			return new Map<T> () {
				arse = new Dictionary<string, T>(this.arse)
			};
		}
	}
}

