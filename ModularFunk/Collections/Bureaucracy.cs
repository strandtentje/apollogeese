using System;
using System.Threading;

namespace BorrehSoft.Utensils.Collections
{
	/// <summary>
	/// Bureaucracy will enqueue objects from one thread without blocking, and
	/// run a callback for each of those objects in a seperate thread. Basically lines up
	/// your CPU's as a production line, if used snazzily.
	/// </summary>
	public class Bureaucracy<T>
	{
		private WaitingQueue<T> sourceQueue;
		private Thread workerThread;
		private QueuebackCallback taskCallback;

		public delegate void QueuebackCallback(T item);

		/// <summary>
		/// Gets a value indicating whether this <see cref="BorrehSoft.Utensils.Collections.Bureaucracy`1"/> is running.
		/// </summary>
		/// <value>
		/// <c>true</c> if running; otherwise, <c>false</c>.
		/// </value>
		public bool Running { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Utensils.Collections.Bureaucracy`1"/> class.
		/// </summary>
		/// <param name='taskCallback'>
		/// Task callback.
		/// </param>
		public Bureaucracy(QueuebackCallback taskCallback, bool autostart = false)
		{
			this.sourceQueue = new WaitingQueue<T>();
			this.workerThread = new Thread(Worker);
			this.taskCallback = taskCallback;
			this.Running = false;
			if (autostart) 
				this.Start();
		}

		/// <summary>
		/// Start the little bureaucracy machine
		/// </summary>
		public void Start ()
		{
			if (!Running) {
				Running = true;
				workerThread.Start();
			}
		}

		/// <summary>
		/// Enqueue the specified item for processing.
		/// </summary>
		/// <param name='item'>
		/// Item.
		/// </param>
		public void Enqueue (T item)
		{
			sourceQueue.Enqueue(item);
		}

		/// <summary>
		/// Stop this instance.
		/// </summary>
		public void Stop ()
		{
			Running = false;
		}

		private void Worker()
		{
			while (Running) taskCallback(sourceQueue.Dequeue());
		}
	}
}

