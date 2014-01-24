using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace BorrehSoft.Utensils
{
	/// <summary>
	/// List
	/// </summary>
	public class List<T> : System.Collections.Generic.List<T>
	{
		public List()
		{

		}

		public List(System.Collections.Generic.IEnumerable<T> newContents) : base(newContents)
		{

		}

		/// <summary>
		/// Turns list into a string array, even if the list doesn't 
		/// contain strings.
		/// </summary>
		/// <returns>
		/// The string array.
		/// </returns>
		public string[] ToStringArray ()
		{
			List<string> sarray = new List<string> ();

			foreach (T item in this)
				sarray.Add (item.ToString ());

			return sarray.ToArray ();
		}
	}

}

