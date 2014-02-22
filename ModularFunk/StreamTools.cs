using System;
using System.IO;
using System.Text;
using System.Net;
using System.Web;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections;

namespace BorrehSoft.Utensils
{
	public static class HttpInterations
	{
		public static void WriteHTML(HttpListenerResponse response, string strData)
		{
			byte[] data = Encoding.ASCII.GetBytes (strData);
			response.ContentEncoding = Encoding.ASCII;
			response.ContentType = "text/html";
			response.ContentLength64 = data.Length;
			response.OutputStream.Write (data, 0, data.Length);
		}

		public static void WriteFile(HttpListenerResponse response, FileInfo file, string mime = "application/octet-stream")
		{
			response.ContentLength64 = file.Length;
			response.ContentType = mime;

			FileStream fileData = file.OpenRead ();
			fileData.CopyTo (response.OutputStream);
			fileData.Close();
		}
	
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

