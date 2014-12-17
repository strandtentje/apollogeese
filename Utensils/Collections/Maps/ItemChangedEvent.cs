using System;

namespace BorrehSoft.Utensils.Collections.Maps
{
	/// <summary>
	/// Item changed event handler.
	/// </summary>
	public delegate void ItemChangedEventHandler<T> (object sender, ItemChangedEventArgs<T> e);

	/// <summary>
	/// Item changed event arguments.
	/// </summary>
	public class ItemChangedEventArgs<T>
	{
		/// <summary>
		/// Gets the name of the item.
		/// </summary>
		/// <value>The name of the item.</value>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the previous value.
		/// </summary>
		/// <value>The previous value.</value>
		public T PreviousValue { get; private set; }

		/// <summary>
		/// Gets the new value.
		/// </summary>
		/// <value>The new value.</value>
		public T NewValue { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Utensils.ItemChangedEventArgs`1"/> class.
		/// </summary>
		/// <param name="PreviousValue">Previous value.</param>
		/// <param name="NewValue">New value.</param>
		public ItemChangedEventArgs (string Name, T PreviousValue, T NewValue)
		{
			this.Name = Name;
			this.PreviousValue = PreviousValue;
			this.NewValue = NewValue;
		}
	}
}

