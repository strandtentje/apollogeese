using System;
using System.Collections.Generic;
using System.Net;
using BorrehSoft.Utensils;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Collections.Maps;
using System.Text;
using System.Threading;
using System.Collections.Specialized;
using System.Diagnostics;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public class ProfilerHog
	{
		public ProfilerHog() {
			this.CorrectForNesting = true;
		}

		long ticksSpent = 0;

		long measurementsMade = 0;

		public long TotalTicksSpent { get { return this.ticksSpent; } }

		public long MeasurementCount { get { return this.measurementsMade; } }

		public long TicksPerMeasurement { get { return this.TotalTicksSpent / Math.Max(1, this.MeasurementCount); } }

		private static ListDictionary threadStopwatches = new ListDictionary ();

		public bool CorrectForNesting { get; set; }

		public void Measure(Action runMeasurementSubject) {

			int threadId = Thread.CurrentThread.ManagedThreadId;
			bool unstackProfiler = threadStopwatches.Contains (threadId) && this.CorrectForNesting;
			Stopwatch previousStopwatch = null;
			Stopwatch stopwatch = new Stopwatch ();

			if (unstackProfiler) {
				previousStopwatch = (Stopwatch)threadStopwatches [threadId];
				previousStopwatch.Stop ();
				threadStopwatches.Remove (threadId);
			}

			if (this.CorrectForNesting) {
				threadStopwatches.Add (threadId, stopwatch);
			}

			stopwatch.Start ();
			runMeasurementSubject ();
			stopwatch.Stop ();

			if (this.CorrectForNesting) {
				threadStopwatches.Remove (threadId);
			}

			if (unstackProfiler) {
				threadStopwatches.Add (threadId, previousStopwatch);
				previousStopwatch.Start ();
			}

			Interlocked.Add (ref ticksSpent, stopwatch.ElapsedTicks);
			Interlocked.Increment (ref measurementsMade);
		}
	}
}
