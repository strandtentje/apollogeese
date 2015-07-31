using System;
using System.IO;
using System.Collections.Generic;

namespace BorrehSoft.Utensils
{
	public class Header : Row
	{
		public Header (Table table, IEnumerable<string> header) : base(table, header)
		{
		}
	}
}

