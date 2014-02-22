using System;

namespace BorrehSoft.Utensils.Collections.Maps
{
	/// <summary>
	/// A regular map, but it raises an event when an item is changed.
	/// </summary>
	public class WatchableMap<T> : Map<T>
	{
		public event ItemChangedEventHandler<T> ItemChanged;

		/// <summary>
		/// Gets or sets the item with the specified name and raises
		/// the ItemChanged event in case of a set.
		/// </summary>
		/// <param name="name">Name</param>
		new public T this[string name] {
			get {
				return base [name];
			}
			set {
				T PreviousValue = base [name];
				T NewValue = value;

				base [name] = value;

				if (ItemChanged == null)
					return;

				ItemChanged (this, 
				            new ItemChangedEventArgs<T> (
					name, PreviousValue, NewValue));
			}
		}
	}
}

