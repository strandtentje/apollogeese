using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace BorrehSoft.Utensils.Log.Profiling
{
	public class Report
	{
		Dictionary<string, Entry> Entries = new Dictionary<string, Entry>();
		public List<Entry> Results { get; private set; }

		public Entry Get (string name)
		{
			if (Entries.ContainsKey (name)) {
				return Entries [name];
			} else {
				Entry newEntry = new Entry (name);
				Entries.Add (name, newEntry);
				return newEntry;
			}
		}

		public void Finalize (long totaltime)
		{
			List<Entry> unsortedResults = new List<Entry>(Entries.Values);
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

