using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace BorrehSoft.Utensils.Log.Profiling
{
	public class Profiler
	{
		private Stopwatch stopwatch = new Stopwatch();

		private Entry Current;
		private Report Entries = new Report ();

		public Profiler() {
			stopwatch.Start ();
		}

		public Report FinalizeIntoReport() {
			stopwatch.Stop ();
			Entries.Finalize (stopwatch.ElapsedTicks);
			return Entries;
		}

		public void CheckIn(string name, Action action)
		{
			Entry previous = Current;

			if (previous != null)
				previous.Stop (stopwatch.ElapsedTicks);

			Current = Entries.Get (name);
			Current.Start (stopwatch.ElapsedTicks);

			action.Invoke ();

			Current.Stop (stopwatch.ElapsedTicks);

			if (previous != null)
				previous.Start (stopwatch.ElapsedTicks);

			Current = previous;
		}
	}
}

