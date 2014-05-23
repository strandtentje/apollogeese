using System;
using System.Threading;
using System.Collections.Generic;

namespace BorrehSoft.Utensils.Collections
{
	/// <summary>
	/// Waiting queue; A queue that doesn't error when it's empty,
	/// but blocks instead, until a new item has been enqueued.
	/// </summary>
	public class WaitingQueue<T>
	{
		EventWaitHandle queueHolder = new EventWaitHandle(false, EventResetMode.AutoReset);
		Queue<T> underlying = new Queue<T>();

		public WaitingQueue ()
		{
		}

		/// <summary>
		/// Dequeue an item.
		/// </summary>
		public T Dequeue ()
		{
			T outItem;

			queueHolder.WaitOne ();
			lock (underlying) {
				outItem = underlying.Dequeue ();
				if (underlying.Count > 0)
					queueHolder.Set();
			}

			return outItem;
		}		

		/// <summary>
		/// Enqueue the specified inItem.
		/// </summary>
		/// <param name='inItem'>
		/// In item.
		/// </param>
		public void Enqueue (T inItem)
		{
			lock (underlying) {
				underlying.Enqueue(inItem);
				queueHolder.Set();
			}
		}


	}
}

