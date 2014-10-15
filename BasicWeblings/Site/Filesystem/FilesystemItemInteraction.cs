using System;
using System.IO;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Duckling.Http;
using BorrehSoft.Utensils.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Filesystem
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

		public bool GenerateKeywords { get; set; }

		public FilesystemItemInteraction (string rootPath, string file)
		{
			this["rootfilesystem"] = rootFilesystem = rootPath;
			this["fullpath"] = fullPath = file;
			this["relativepath"] = relativePath = fullPath.Remove(0, rootFilesystem.Length);
		}

		public FilesystemItemInteraction (string baseUrl, string rootFilesystem, bool generateKeywords = true)
		{
			this.rootFilesystem = rootFilesystem;
			this.baseUrl = baseUrl;
			this.GenerateKeywords = generateKeywords;
		}

		public FilesystemItemInteraction (IInteraction parent, string rootFilesystem, string originalRequest, bool generateKeywords = false) : base(parent)
		{
			this.rootFilesystem = rootFilesystem;
			this.baseUrl = originalRequest.Trim('/');
			this.GenerateKeywords = generateKeywords;
		}		

		static List<string> KeywordsFromString (string name)
		{
			List<string> keywords = new List<string>();

			foreach(Match match in keywordMatcher.Matches(name))
				keywords.Add(match.Value);

			return keywords;
		}

		public void Assume (FileSystemInfo info)
		{
			this["rootfilesystem"] = rootFilesystem;
			this["name"] = name = info.Name;

			if (GenerateKeywords)
				this["keywords"] = KeywordsFromString(name);

			string url;

			if (baseUrl.Length > 0)
				url = string.Format("/{0}/{1}", baseUrl, name);
			else
				url = "/" + name;

			this["url"] = url; 
			this["fullpath"] = fullPath =  info.FullName;
			this["relativepath"] = relativePath = fullPath.Remove(0, rootFilesystem.Length);

			FileInfo fileInfo = info as FileInfo;

			if (fileInfo != null) 
				this["filesize"] = Filesize(fileInfo.Length);
			else 
				this["filesize"] = "-";

		}

		static string[] suffices = new string[] {
			"B", "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"
		};

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

