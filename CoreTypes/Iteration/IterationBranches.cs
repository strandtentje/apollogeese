using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Collections.Generic;

namespace Iteration
{
	public class IterationBranches
	{
		public Service None, First, Single, Iterator, Last;
		public Dictionary<int, Service> ItemBranches = new Dictionary<int, Service> ();

		public bool AddBranch (string name, Service handler)
		{
			if (name == "none") {
				None = handler;
			} else if (name == "first") {
				First = handler;
			} else if (name == "single") {
				Single = handler;
			} else if (name == "iterator") {
				Iterator = handler;
			} else if (name == "last") {
				Last = handler;
			} else if (name.StartsWith ("item_")) {
				ItemBranches.Add (int.Parse (name.Remove (name.Length - "item_".Length)), handler);
			} else { 
				return false;
			}
			return true;
		}
	}
}

