using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;
using System.Net;
using BorrehSoft.Utensils;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BorrehSoft.Extensions.FileServer
{
	class Mapping
	{
		static char slash = Path.DirectorySeparatorChar;

		public string URL { get; set; }
		public string FileSystemPath { get; set; }
		public Settings Whitelist { get; set; }
		public bool AllowBrowsing { get; set; }

		public string ListMarkup { get; set; }
		public string DirectoryItemMarkup { get; set; }
		public string FileItemMarkup { get; set; }

		public Mapping (Settings s)
		{
			URL = (string)s ["url"];
			FileSystemPath = ((string)s ["filesystempath"]).TrimEnd (slash) + slash;
			Whitelist = (Settings)s ["whitelist"];
			AllowBrowsing = (bool)s ["allowbrowsing"];

			ListMarkup = (string)s ["listmarkup"];
			DirectoryItemMarkup = (string)s ["directorymarkup"];
			FileItemMarkup = (string)s ["filemarkup"];
		}

		public bool Follow (string rawUrl, HttpListenerResponse response)
		{
			// Failure when this mapping isn't relevant
			if (!rawUrl.StartsWith (URL)) return false;

			string tail = rawUrl.Substring (URL.Length);
			string fsPath = FileSystemPath + tail.TrimStart ('/');

			if (Directory.Exists (fsPath)) {
				DirectoryInfo info = new DirectoryInfo (fsPath);
				if (!AllowBrowsing || !info.Exists)
					return false;

				StringBuilder fileList = new StringBuilder ();

				DirectoryInfo[] dirs = info.GetDirectories ();
				FileInfo[] files = info.GetFiles ();

				foreach (DirectoryInfo dir in dirs) 
					if (!dir.Name.StartsWith("."))
						fileList.AppendLine (string.Format (
							DirectoryItemMarkup, 
							rawUrl + dir.Name, 
							dir.Name));

				foreach (FileInfo file in files)
					if (!file.Name.StartsWith("."))
						fileList.AppendLine (string.Format (
							FileItemMarkup, rawUrl + file.Name, file.Name));

				string fileListHTML = string.Format (ListMarkup, fileList.ToString ());

				response.ContentType = "text/html";
				response.ContentLength64 = fileListHTML.Length;
				response.ContentEncoding = Encoding.ASCII;
				response.OutputStream.Write (Encoding.ASCII.GetBytes (fileListHTML), 0, fileListHTML.Length);

				return true;

			} else if (File.Exists (fsPath)) {
				FileInfo info = new FileInfo (fsPath);
				string mimeType = (string)Whitelist [info.Extension.TrimStart('.')];
				if (mimeType == null)
					return false;

				response.ContentType = mimeType;
				response.ContentLength64 = info.Length;

				FileStream fileData = info.OpenRead ();
				fileData.CopyTo (response.OutputStream);
				fileData.Close ();

				return true;
			}

			return false;
		}
	}
}