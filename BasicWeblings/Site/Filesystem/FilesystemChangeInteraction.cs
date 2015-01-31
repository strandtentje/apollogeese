using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.IO;
using BorrehSoft.Utensils.Collections;
using System.Text.RegularExpressions;
using BorrehSoft.Utensils.Collections.Maps.Search;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Filesystem
{
	/// <summary>
	/// Used when a change in the filesystem occured
	/// </summary>
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

		/// <summary>
		/// Keywords associated with this file
		/// </summary>
		/// <value>The keywords.</value>
		public string[] Keywords { 
			get { 
				return this["keywords"] as string[];
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is a directory.
		/// </summary>
		/// <value><c>true</c> if this instance is directory; otherwise, <c>false</c> if it's a file.</value>
		public bool IsDirectory { 
			get {
				return (bool)this["isdirectory"];
			}
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="BorrehSoft.Extensions.BasicWeblings.Site.Filesystem.FilesystemChangeInteraction"/> class.
		/// </summary>
		/// <param name="info">File info</param>
		/// <param name="keywords">Keywords this file associates with</param>
		/// <param name="rootPath">Root that was being watched</param>
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

