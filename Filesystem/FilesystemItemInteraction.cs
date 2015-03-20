using System;
using System.IO;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Http;
using BorrehSoft.Utensils.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BorrehSoft.ApolloGeese.Extensions.Filesystem
{
	/// <summary>
	/// Filesystem item interaction.
	/// A rather ugly bastard. 
	/// Good thing it's not 3000 lines of it.
	/// </summary>
	class FilesystemItemInteraction : QuickInteraction
	{	
		static Regex keywordMatcher = new Regex("[a-z]+|[0-9]+");

		string rootFilesystem, name, fullPath, baseUrl, relativePath;

		/// <summary>
		/// This instance will generate keywords for each new file assumed, when 
		/// this is set to true.
		/// </summary>
		/// <value><c>true</c> if generating keywords; otherwise, <c>false</c>.</value>
		public bool GenerateKeywords { get; set; }

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="BorrehSoft.Extensions.BasicWeblings.Site.Filesystem.FilesystemItemInteraction"/> class,
		/// for a file in a directory.
		/// </summary>
		/// <param name="rootPath">Root path.</param>
		/// <param name="file">File.</param>
		public FilesystemItemInteraction (string rootPath, string file)
		{
			this["rootfilesystem"] = rootFilesystem = rootPath;
			this["fullpath"] = fullPath = file;
			this["relativepath"] = relativePath = fullPath.Remove(0, rootFilesystem.Length);
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="BorrehSoft.Extensions.BasicWeblings.Site.Filesystem.FilesystemItemInteraction"/> class,
		/// for an URL and the root folder.
		/// </summary>
		/// <param name="baseUrl">Base URL.</param>
		/// <param name="rootFilesystem">Root filesystem.</param>
		/// <param name="generateKeywords">If set to <c>true</c> generate keywords.</param>
		public FilesystemItemInteraction (string baseUrl, string rootFilesystem, bool generateKeywords = true)
		{
			this.rootFilesystem = rootFilesystem;
			this.baseUrl = baseUrl;
			this.GenerateKeywords = generateKeywords;
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="BorrehSoft.Extensions.BasicWeblings.Site.Filesystem.FilesystemItemInteraction"/> class,
		/// for a request and the root folder, using a parent interaction.
		/// </summary>
		/// <param name="parent">Parent.</param>
		/// <param name="rootFilesystem">Root filesystem.</param>
		/// <param name="originalRequest">Original request.</param>
		/// <param name="generateKeywords">If set to <c>true</c> generate keywords.</param>
		public FilesystemItemInteraction (IInteraction parent, string rootFilesystem, string originalRequest, bool generateKeywords = false) : base(parent)
		{
			this.rootFilesystem = rootFilesystem;
			this.baseUrl = originalRequest.Trim('/');
			this.GenerateKeywords = generateKeywords;
		}		

		/// <summary>
		/// Turns string into a bunch of keywords
		/// </summary>
		/// <returns>The keywords</returns>
		/// <param name="name">string</param>
		static List<string> KeywordsFromString (string name)
		{
			List<string> keywords = new List<string>();

			foreach(Match match in keywordMatcher.Matches(name))
				keywords.Add(match.Value);

			return keywords;
		}

		/// <summary>
		/// Assume new fileinfo, overwriting the currently represented file.
		/// </summary>
		/// <param name="info">Info.</param>
		public void Assume (FileSystemInfo info)
		{
			this ["rootfilesystem"] = rootFilesystem;
			this ["name"] = name = info.Name;
			if ((info is FileInfo) && (info.Extension.Length > 0)) {
				this ["shortname"] = info.Name.Remove (info.Name.Length - info.Extension.Length);
				this ["extension"] = info.Extension.TrimStart ('.');
			}

			if (GenerateKeywords)
				this ["keywords"] = KeywordsFromString (name);

			string url;

			if (baseUrl.Length > 0) {
				url = string.Format ("/{0}/{1}", baseUrl, name);
				this ["parent"] = string.Format ("/{0}", baseUrl);
			} else {
				url = "/" + name;
			}

			this["url"] = url; 
			this["fullpath"] = fullPath =  info.FullName;
			this["relativepath"] = relativePath = fullPath.Remove(0, rootFilesystem.Length);

			FileInfo fileInfo = info as FileInfo;

			if (fileInfo != null) 
				this["filesize"] = Filesize(fileInfo.Length);
			else 
				this["filesize"] = "-";

		}

		/// <summary>
		/// Filesize suffices.
		/// </summary>
		static string[] suffices = new string[] {
			"B", "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"
		};

		/// <summary>
		/// Takes a bytecount and turns it into a more humanreadable filesize.
		/// </summary>
		/// <param name="count">Count.</param>
	    static string Filesize (long count)
		{
			double approxSize = (double)count;
			int suffix = 0;

			while (
				(approxSize > 1000) && 
				(suffix < suffices.Length - 1)) {
				approxSize /= 1000;
				suffix++;
			}

			return string.Format("{0} {1}", approxSize.ToString("0.0"), suffices[suffix]);
		}
	}

}

