using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;
using System.Net;
using BorrehSoft.Utensils;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.Extensions.FileServer
{
	/// <summary>
	/// A Mapping from URL to filesystem.
	/// </summary>
	class Mapping
	{
		static char slash = Path.DirectorySeparatorChar;

		/// <summary>
		/// Gets or sets the URL which this fileserver maps to
		/// </summary>
		/// <value>The URL</value>
		public string URL { get; set; }
		/// <summary>
		/// Gets or sets the file system path this fileserver maps from
		/// </summary>
		/// <value>The file system path.</value>
		public string FileSystemPath { get; set; }
		/// <summary>
		/// Gets or sets the whitelist of mime-types
		/// </summary>
		/// <value>The whitelist.</value>
		public Settings Whitelist { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="BorrehSoft.Extensions.FileServer.Mapping"/> allow folder browsing.
		/// </summary>
		/// <value><c>true</c> if allow browsing; otherwise, <c>false</c>.</value>
		public bool AllowBrowsing { get; set; }

		/// <summary>
		/// Gets or sets the list HTML.
		/// </summary>
		/// <value>The list markup.</value>
		public string ListMarkup { get; set; }
		/// <summary>
		/// Gets or sets the directory item HTML.
		/// </summary>
		/// <value>The directory item markup.</value>
		public string DirectoryItemMarkup { get; set; }
		/// <summary>
		/// Gets or sets the file item HTML.
		/// </summary>
		/// <value>The file item markup.</value>
		public string FileItemMarkup { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Extensions.FileServer.Mapping"/> class.
		/// </summary>
		/// <param name="s">Configuration settings for this mapping</param>
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

		/// <summary>
		/// Follow the specified URL and respond with a blank page, index or file.
		/// </summary>
		/// <param name="rawUrl">Raw URL.</param>
		/// <param name="response">Response.</param>
		public bool Follow (Interaction parameters)
		{
			string rawUrl = parameters.RawURL;

			// Failure when this mapping isn't relevant
			if (!rawUrl.StartsWith (URL)) return false;
			
			Secretary.Report (8, "Path requested: ", rawUrl);

			string 
				tail = rawUrl.Substring (URL.Length),
				fsPath = FileSystemPath + tail.TrimStart ('/');

			if (Directory.Exists (fsPath))
				return GetDirectoryIndex (parameters, rawUrl, fsPath);
			else if (File.Exists (fsPath)) 
				return GetFileContents (parameters, rawUrl, fsPath);

			parameters.StatusCode = 404;

			return false;
		}

		/// <summary>
		/// Gets the index of the directory.
		/// </summary>
		/// <returns><c>true</c>, if directory index was gotten, <c>false</c> otherwise.</returns>
		/// <param name="response">Response.</param>
		/// <param name="fsPath">Filesystem path.</param>
		/// <param name="rawUrl">Raw Requesy URL.</param>
		bool GetDirectoryIndex (Interaction parameters, string rawUrl, string fsPath)
		{
			rawUrl = rawUrl.TrimEnd ('/') + '/';

			DirectoryInfo info = new DirectoryInfo (fsPath);

			if (!AllowBrowsing) { parameters.StatusCode = 403; return false; }
			if (!info.Exists) { parameters.StatusCode = 404; return false; }

			StringBuilder fileList = new StringBuilder ();

			DirectoryInfo[] dirs = info.GetDirectories ();
			FileInfo[] files = info.GetFiles ();

			foreach (DirectoryInfo dir in dirs) 
				if (!dir.Name.StartsWith("."))
					fileList.AppendLine (string.Format (
						DirectoryItemMarkup, 
						HttpUtility.UrlEncode(rawUrl + dir.Name).Replace("%2f", "/"), 
						dir.Name));

			foreach (FileInfo file in files)
				if (!file.Name.StartsWith("."))
					fileList.AppendLine (string.Format (
						FileItemMarkup, 
						HttpUtility.UrlEncode(rawUrl + file.Name).Replace("%2f", "/"), 
						file.Name));

			parameters.HTML.AppendLine (
				string.Format (ListMarkup, fileList.ToString ()));

			Secretary.Report (9, "Was directory, providing index.");

			return true;
		}

		/// <summary>
		/// Gets the file contents.
		/// </summary>
		/// <returns><c>true</c>, if file contents was gotten, <c>false</c> otherwise.</returns>
		/// <param name="response">Response.</param>
		/// <param name="fsPath">Filesystem path.</param>
		bool GetFileContents (Interaction parameters, string rawUrl, string fsPath)
		{
			FileInfo info = new FileInfo (fsPath);

			string mimeType = (string)Whitelist [info.Extension.TrimStart ('.')];
			if (mimeType == null) {
				parameters.StatusCode = 404;
				return false;
			}
			
			FileInfo file = new FileInfo (fsPath);

			parameters.MimeType = mimeType;
			parameters.Size = file.Length;
			parameters.BodyInStream = file.OpenRead ();

			Secretary.Report (9, "Was file, providing content.");

			return true;
		}
	}
}