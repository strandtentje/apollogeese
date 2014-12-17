using System;
using System.Diagnostics;
using BorrehSoft.Utensils.Collections;
using System.Collections.Generic;

namespace BorrehSoft.Utensils.Log.Profiling
{
	public class Report
	{
		Map<Entry> Entries = new Map<Entry>();
		public List<Entry> Results { get; private set; }

		public Entry Get (string name)
		{
			return Entries.GetOrKeep (name, delegate() {
				return new Entry(name);
			});
		}

		public void Finalize (long totaltime)
		{
			List<Entry> unsortedResults = new List<Entry>(Entries.Dictionary.Values);
			unsortedResults.Sort ();
			Results = unsortedResults;

			foreach (Entry entry in Results)
				entry.Finalize (totaltime);
		}

		public override string ToString ()
		{
			return string.Format ("Log.Profiling.Report \n{0}", string.Join ("\n", Results));
		}
	}
}

