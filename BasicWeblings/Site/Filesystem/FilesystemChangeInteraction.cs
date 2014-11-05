using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.IO;
using BorrehSoft.Utensils.Collections;
using System.Text.RegularExpressions;
using BorrehSoft.Utensils.Collections.Maps.Search;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Filesystem
{
	public class FilesystemChangeInteraction : QuickInteraction
	{
		FileSystemInfo info;

		/// <summary>
		///  Gets or sets the filename. 
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { 
			get {
				return this.GetString("name", null);
			}
		}

		/// <summary>
		/// Gets the full name.
		/// </summary>
		/// <value>
		/// The full name.
		/// </value>
		public string FullName { 
			get {
				return this.GetString("fullname", null); 
			}
		}

		public string[] Keywords { 
			get { 
				return this["keywords"] as string[];
			}
		}

		public bool IsDirectory { 
			get {
				return (bool)this["isdirectory"];
			}
		}

		public FilesystemChangeInteraction (FileSystemInfo info, string[] keywords, string rootPath = "")
		{
			this.info = info;	
			this ["name"] = info.Name;
			this ["fullname"] = info.FullName;
			if (info.FullName.StartsWith (rootPath)) {
				string url = info.FullName.Remove (0, rootPath.Length);
				this ["url"] = url;
				this ["parent"] = url.Remove(url.Length - info.Name.Length);
			}
			this["keywords"] = keywords;
			this["isdirectory"] = (this.info.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
		}
	}
}

