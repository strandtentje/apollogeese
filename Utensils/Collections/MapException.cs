using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BorrehSoft.Utensils.Collections
{
	public class MapException : Exception
	{
		public MapException (string index, string message) : base(string.Format("{0} {1}", index, message))
		{

		}
	}
}
