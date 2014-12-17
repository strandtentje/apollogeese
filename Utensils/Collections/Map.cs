using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BorrehSoft.Utensils.Collections
{
	/// <summary>
	/// Easy to use map using the this[] property.
	/// </summary>
	public class Map<T> 
	{
		public Map() { 
		
		}

		public Map(Map<T> origin) {
			this.backEnd = origin.backEnd;
		}

		/// <summary>
		/// Gets the dictionary.
		/// </summary>
		/// <value>
		/// The dictionary.
		/// </value>
		public virtual Dictionary<string, T> Dictionary {
			get {
				return backEnd;
			}
		}

		/// <summary>
		/// The arse.
		/// </summary>
		private Dictionary<string, T> backEnd = new Dictionary<string, T> ();
	
		/// <summary>
		/// Determines whether this instance has the specified key.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance has the specified key; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='key'>
		/// If set to <c>true</c> key.
		/// </param>
		public virtual bool Has(string key) {
			return backEnd.ContainsKey(key);
		}

		/// <summary>
		/// Deletes the key.
		/// </summary>
		/// <param name='key'>
		/// Key.
		/// </param>
		protected virtual void Delete (string key)
		{
			backEnd.Remove(key);
		}

		/// <summary>
		/// Adds the item with specified key.
		/// </summary>
		/// <param name='key'>
		/// Key.
		/// </param>
		/// <param name='value'>
		/// Value.
		/// </param>
		protected virtual void Add(string key, T value)
		{
			backEnd.Add(key, value);
		}

		/// <summary>
		/// Get item at the specified key.
		/// </summary>
		/// <param name='key'>
		/// Key.
		/// </param>
		public virtual T Get(string key)
		{
			return backEnd[key];
		}

		public T Get(string key, T defaultValue)
		{
			if (backEnd.ContainsKey(key))
				return backEnd[key];
			else
				return defaultValue;
		}

		public T GetOrKeep(string key, Func<T> defaultFunction)
		{
			if (backEnd.ContainsKey (key)) {
				return backEnd [key];
			}
			else {
				T newt = defaultFunction ();
				backEnd.Add (key, newt);
				return newt;
			}
		}

		/// <summary>
		/// Gets or sets the item with the specified name.
		/// </summary>
		/// <param name="name">Name</param>
		public T this[string name]
		{
			get	{
				if (this.Has(name))
					return this.Get(name);
				return default(T);
			}
			set {
				if (this.Has(name))
					this.Delete(name);
				this.Add (name, value);
			}
		}

		/// <summary>
		/// Gets the string.
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="name">Name.</param>
		/// <param name="defaultValue">Default value.</param>
		public string GetString(string name, string defaultValue) 
		{
			// Jesus fuck C# that's sexy
			return (this [name] as string) ?? defaultValue;
		}

		/// <summary>
		/// Tries to get a value from the map
		/// </summary>
		/// <returns>
		/// The gotten value.
		/// </returns>
		/// <param name='name'>
		/// If set to <c>true</c> name.
		/// </param>
		/// <param name='chunk'>
		/// If set to <c>true</c> chunk.
		/// </param>
		public bool TryGetValue (string name, out object chunk)
		{			
			chunk = this[name];

			if (chunk == null) return false;

			return true;
		}

		/// <summary>
		/// Tries to get a string from the map.
		/// </summary>
		/// <returns><c>true</c>, if get string was found and returned, <c>false</c> otherwise.</returns>
		/// <param name="name">Name of the map entry.</param>
		/// <param name="chunk">Value of the map entry.</param>
		public bool TryGetString (string name, out string chunk) 
		{
			chunk = "";

			if (this [name] == null)
				return false;

			if (this [name] is string) {
				// You disappoint me C#
				chunk = (string)(object)this [name];
				return true;
			}

			return false;
		}

		/// <summary>
		/// Clone this instance.
		/// </summary>
		public Map<T> Clone ()
		{
			return new Map<T> () {
				backEnd = new Dictionary<string, T>(this.backEnd)
			};
		}
	}
}
