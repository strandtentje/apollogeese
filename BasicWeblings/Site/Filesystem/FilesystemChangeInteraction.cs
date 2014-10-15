using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.IO;
using BorrehSoft.Utensils.Collections;
using System.Text.RegularExpressions;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Filesystem
{
	public class FilesystemChangeInteraction : QuickInteraction
	{
		FileInfo info;
		static Regex alphaNumerical = new Regex(@"\W|_");

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

		public FilesystemChangeInteraction (string fullPath)
		{
			info = new FileInfo(fullPath);
			this["name"] = info.Name;
			this["fullname"] = info.FullName;
			this["keywords"] = alphaNumerical.Split(info.Name.ToLower());
		}
	}
}

