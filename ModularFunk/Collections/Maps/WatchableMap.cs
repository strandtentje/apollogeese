using System;
using System.ComponentModel;

namespace BorrehSoft.Utensils.Collections.Maps
{
	/// <summary>
	/// A regular map, but it raises an event when an item is changed.
	/// </summary>
	public class WatchableMap<T> : Map<T>, INotifyPropertyChanged
	{
		public WatchableMap() : base() {}

		public WatchableMap(Map<T> origin) : base(origin) {}

		public event ItemChangedEventHandler<T> ItemChanged;

		public event PropertyChangedEventHandler PropertyChanged;

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

				if (ItemChanged != null)
					ItemChanged (this, 
					            new ItemChangedEventArgs<T> (
						name, PreviousValue, NewValue));

				if (PropertyChanged != null)
					PropertyChanged(this, 
					                new PropertyChangedEventArgs(
						name));

			}
		}

	}
}

