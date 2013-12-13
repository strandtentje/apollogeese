using System;

namespace BorrehSoft.Utensils
{
	/// <summary>
	/// A Tuple, of any two types.
	/// </summary>
	public class Tuple<T1, T2>
	{
		/// <summary>
		/// Gets or sets the key-portion of the tuple.
		/// </summary>
		/// <value>
		/// The key.
		/// </value>
		public T1 Key { get; set; }
		/// <summary>
		/// Gets or sets the value-portion of the tuple
		/// </summary>
		/// <value>
		/// The value.
		/// </value>
		public T2 Value { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Utensils.Tuple`2"/> class.
		/// </summary>
		/// <param name='Key'>
		/// Key.
		/// </param>
		/// <param name='Value'>
		/// Value.
		/// </param>
		public Tuple (T1 Key, T2 Value)
		{
			this.Key = Key;
			this.Value = Value;
		}
	}
}

