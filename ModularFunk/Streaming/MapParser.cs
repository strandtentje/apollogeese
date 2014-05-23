using System;
using System.IO;
using BorrehSoft.Utensils.Collections;
using System.Text;
using System.Collections.Generic;
using System.Web;

namespace BorrehSoft.Utensils
{
	public class MapParser
	{
		public static void ReadIntoMap(Stream source, char seperator, char concatenator, ref Map<string> target)
		{
			StringBuilder buffer = new StringBuilder ();
			Queue<string> queue = new Queue<string>();

			while(source.Position < source.Length)
			{
				int inByte = source.ReadByte();

				if ((char)inByte == concatenator)
				{
					if (queue.Count == 1) target[queue.Dequeue()] = "";
					if (queue.Count == 2) target[queue.Dequeue()] = queue.Dequeue();
					queue.Clear();
				} else if ((char)inByte == seperator)
				{
					queue.Enqueue(HttpUtility.UrlDecode(buffer.ToString()));
					buffer.Clear();
				} else {
					buffer.Append ((char)inByte);
				}
			}
		}
	}
}

