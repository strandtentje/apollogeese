using System;
using System.Text;

namespace BorrehSoft.Utensils
{
	public class List<T> : System.Collections.Generic.List<T>
	{
		public List()
		{

		}

		public List(System.Collections.Generic.IEnumerable<T> newContents) : base(newContents)
		{

		}

		public string[] ToStringArray ()
		{
			List<string> sarray = new List<string> ();

			foreach (T item in this)
				sarray.Add (item.ToString ());

			return sarray.ToArray ();
		}
	}
}

