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
		public class Entry {
			public Entry(T Item)
			{
				this.Item = Item;
			}
			public T Item;
			public Semaphore Acquired = new Semaphore(0, 1);
			public Thread OwningThread;
		}

		EventWaitHandle queueHolder = new EventWaitHandle(false, EventResetMode.AutoReset);
		Queue<Entry> underlying = new Queue<Entry>();

		public WaitingQueue ()
		{
		}

		/// <summary>
		/// Dequeue an item.
		/// </summary>
		public T Dequeue ()
		{
			Entry outItem;

			queueHolder.WaitOne ();
			lock (underlying) {
				outItem = underlying.Dequeue ();
				outItem.OwningThread = Thread.CurrentThread;
				outItem.Acquired.Release();

				if (underlying.Count > 0)
					queueHolder.Set();
			}

			return outItem.Item;
		}			

		/// <summary>
		/// Enqueue the specified inItem.
		/// </summary>
		/// <param name='inItem'>
		/// In item.
		/// </param>
		public Entry Enqueue (T inItem)
		{
			Entry newEntry = new Entry(inItem);

			lock (underlying) {
				underlying.Enqueue(newEntry);
				queueHolder.Set();
			}

			return newEntry;
		}

		public Thread WaitEnqueue(T inItem)
		{
			Entry entry = Enqueue(inItem);
			entry.Acquired.WaitOne();
			return entry.OwningThread;
		}
	}
}

