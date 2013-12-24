using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.IO;

namespace BorrehSoft.Utensils.Log
{
	/// <summary>
	/// Secretary.
	/// </summary>
	public class Secretary : IDisposable
	{
		/// <summary>
		/// Gets the latest log.
		/// </summary>
		/// <value>The latest log.</value>
		public static Secretary LatestLog { get; private set; }

		private EventWaitHandle logSignal = new EventWaitHandle (true, EventResetMode.AutoReset);
		private Queue<string[]> messages = new Queue<string[]> ();
		private StringBuilder logEntry = new StringBuilder();
		private StreamWriter fileOut;

		public int globVerbosity = 10;
		public bool running = true;

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Utensils.Secretary"/> class.
		/// </summary>
		/// <param name="file">Target Logfile.</param>
		public Secretary(string file)
		{
			fileOut = new StreamWriter(file) { AutoFlush =  true };

			(new Thread (LogFlusher)).Start ();

			LatestLog = this;
		}

		/// <summary>
		/// During runtime, flushes incoming reports into console window and log file.
		/// </summary>
		private void LogFlusher()
		{
			string tmpChunk;

			lock(fileOut)
			{
				while(running)
				{
					logSignal.WaitOne();
					ConsumeLines (out tmpChunk);
					Console.Write(tmpChunk); fileOut.Write(tmpChunk);
				}

				fileOut.Close();
			}
		}

		/// <summary>
		/// Consumes the unflushed lines.
		/// </summary>
		/// <param name="logChunk">Log chunk.</param>
		private void ConsumeLines(out string logChunk)
		{
			while (messages.Count > 0) {
				string[] message = messages.Dequeue ();							
				logEntry.Append(DateTime.Now.ToString ("HH:MM:SS | "));
				logEntry.AppendLine(string.Join (" ", message));
			}

			logChunk = logEntry.ToString ();
			logEntry.Clear ();
		}

		/// <summary>
		/// Releases all resource used by the <see cref="BorrehSoft.Utensils.Secretary"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="BorrehSoft.Utensils.Secretary"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="BorrehSoft.Utensils.Secretary"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="BorrehSoft.Utensils.Secretary"/>
		/// so the garbage collector can reclaim the memory that the <see cref="BorrehSoft.Utensils.Secretary"/> was occupying.</remarks>
		public void Dispose()
		{
			running = false;
			ReportHere (0, "Logfile Ends Here.");
			lock (fileOut);

			logSignal.Dispose ();
			messages.Clear ();
			fileOut.Dispose ();
		}

		/// <summary>
		/// Report message at most recently spawned secretary.
		/// </summary>
		/// <param name="verbosity">Verbosity.</param>
		/// <param name="messageParts">Message parts.</param>
		public static void Report(int verbosity, params string[] messageParts)
		{
			LatestLog.ReportHere (verbosity, messageParts);
		}

		/// <summary>
		/// Report messageParts with specified verbosity.
		/// </summary>
		/// <param name='verbosity'>
		/// Verbosity level.
		/// </param>
		/// <param name='messageParts'>
		/// Message parts.
		/// </param>
		public void ReportHere (int verbosity, params string[] messageParts)
		{
			if (running && (verbosity <= globVerbosity)) {
				messages.Enqueue (messageParts);
				logSignal.Set ();
			}
		}
	}
}