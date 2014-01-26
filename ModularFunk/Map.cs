using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BorrehSoft.Utensils
{
	/// <summary>
	/// Easy to use map using the this[] property.
	/// </summary>
	public class Map<T>
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		public delegate string Parser(string data);

		/// <summary>
		/// Adds elements from string stream.
		/// </summary>
		/// <param name="data">Data to parse</param>
		/// <param name="parser">String parser for element value.</param>
		/// <param name="assigner">Value-assigning character.</param>
		/// <param name="concatenator">Concatenator of assignments.</param>
		public void AddFromString(string source, Parser parser, char assigner, char concatenator)
		{
			StringBuilder buffer = new StringBuilder ();
			Queue<string> queue = new Queue<string>();

			int inByte;
			string id, data;

			foreach(char c in source)
			{
				if (c == concatenator)
				{
					id = queue.Dequeue(); data = "";

					if (queue.Count > 0) data = parser(queue.Dequeue());

					this[id] = (T)(object)data;

					queue.Clear();
				}
				else if (c == assigner)
				{
					queue.Enqueue(buffer.ToString());
					buffer.Clear();
				}
				else {
					buffer.Append(c);
				}
			}
		}

		/// <summary>
		/// The arse.
		/// </summary>
		private Dictionary<string, T> arse = new Dictionary<string, T> ();

		/// <summary>
		/// Gets the names.
		/// </summary>
		/// <returns>The names.</returns>
		public ICollection<string> GetNames() {
			return arse.Keys;
		}

		/// <summary>
		/// Asserts the presence of an item.
		/// </summary>
		/// <param name='key'>
		/// Key.
		/// </param>
		/// <param name='source'>
		/// Source.
		/// </param>
		public void AssertItem(string key, object source)
		{
			if (!arse.ContainsKey (key))
				throw new KeyNotFoundException (
					string.Format ("{0} expected the key '{1}' in the Map '{2}'", 
				               source.ToString (), key, Name));
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
				arse = new Dictionary<string, T>(this.arse)
			};
		}
	}
}
