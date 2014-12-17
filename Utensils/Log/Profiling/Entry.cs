using System;

namespace BorrehSoft.Utensils.Log.Profiling
{	
	public class Entry : IComparable {
		public bool CheckedIn;
		public string Name;
		public long TotalTime;
		public float Percentage { get; private set; }
		private long startTime;

		public Entry(string name)
		{
			this.Name = name;
		}

		public void Stop (long elapsedTicks)
		{
			TotalTime += elapsedTicks - startTime;
		}

		public void Start (long elapsedTicks)
		{
			startTime = elapsedTicks;
		}

		internal void Finalize(long universeduration)
		{
			Percentage = (float)((double)TotalTime / (double)universeduration);
		}

		public int CompareTo(object obj) {
			int result = 1;

			if (obj != null) {
				Entry other = obj as Entry;
				if (other != null) {
					result = TotalTime.CompareTo (other.TotalTime);
				}
			}

			return result;
		}

		public override string ToString ()
		{
			string 
				percentageString = (Percentage * 100f).ToString ("00.00"),
				barString = (new string ('|', (int)(30 * Percentage))).PadRight (30, '.');

			return string.Format (
				"{3} {0} {1}% [{2}]", 
				TotalTime.ToString().PadLeft(8), 
				percentageString, 
				barString, 
				Name.PadRight(20));
		}
	}
}

