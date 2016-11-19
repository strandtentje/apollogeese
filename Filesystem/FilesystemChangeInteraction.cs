using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using BorrehSoft.Utilities.Collections;
using System.Text.RegularExpressions;
using BorrehSoft.Utilities.Collections.Maps.Search;

namespace BorrehSoft.ApolloGeese.Extensions.Filesystem
{
	/// <summary>
	/// Used when a change in the filesystem occured
	/// </summary>
	public class FilesystemChangeInteraction : SimpleInteraction
	{
		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="BorrehSoft.Extensions.BasicWeblings.Site.Filesystem.FilesystemChangeInteraction"/> class.
		/// </summary>
		/// <param name="info">File info</param>
		/// <param name="keywords">Keywords this file associates with</param>
		/// <param name="rootPath">Root that was being watched</param>
		public FilesystemChangeInteraction (
			FileSystemInfo info, string[] keywords, string rootPath = "",
			IInteraction parent = null) : base (parent)
		{
			this ["name"] = info.Name;
			this ["fullname"] = info.FullName;
			this ["lastdate"] = info.LastWriteTime.ToString ("s", System.Globalization.CultureInfo.InvariantCulture);

			if (info.FullName.StartsWith (rootPath)) {
				string url = info.FullName.Remove (0, rootPath.Length);
				this ["url"] = url;
				this ["parent"] = url.Remove(url.Length - info.Name.Length);
				this ["parenturl"] = this ["parent"];
			}
			this["keywords"] = keywords;
			this["isdirectory"] = (info.Attributes & FileAttributes.Directory) == FileAttributes.Directory;

			FileInfo fileInfo = info as FileInfo;

			if (fileInfo != null) {
				this ["filesize"] = FilesystemItemInteraction.Filesize (fileInfo.Length);
				this ["bytecount"] = fileInfo.Length;

				if (fileInfo.Extension.Length > 0) {
					this ["shortname"] = info.Name.Remove (info.Name.Length - info.Extension.Length);
					this ["extension"] = info.Extension.TrimStart ('.').ToLower();
				}
			} else {
				this["filesize"] = "-";
			}
		}
	}
}

