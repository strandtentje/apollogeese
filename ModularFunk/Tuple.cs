using System;

namespace ModularFunk
{
	public class Tuple<T1, T2>
	{
		public T1 Key { get; set; }
		public T2 Value { get; set; }

		public Tuple (T1 Key, T2 Value)
		{
			this.Key = Key;
			this.Value = Value;
		}
	}
}

