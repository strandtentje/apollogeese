using System;
using System.IO;
using System.Text;
using System.Net;

namespace BorrehSoft.Utensils
{
	public static class Response
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
	}
}

