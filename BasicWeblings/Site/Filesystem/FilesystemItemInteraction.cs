using System;
using System.IO;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Duckling.Http;
using BorrehSoft.Utensils.Collections;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Filesystem
{
	class FilesystemItemInteraction : Map<object>, IInteraction
	{
		IInteraction parent;

		string rootFilesystem, name, fullPath, originalRequest;

		public FilesystemItemInteraction (IInteraction parent, string rootFilesystem, string originalRequest)
		{
			this.parent = parent;
			this.rootFilesystem = rootFilesystem;
			this.originalRequest = originalRequest.Trim('/');
		}		

		public IInteraction Root {
			get {
				return Parent.Root;
			}
		}

		public IInteraction Parent {
			get {
				return this.parent;
			}
		}

		public IInteraction GetClosest (Type t)
		{
			for (IInteraction current = this; (current != null); current = current.Parent) {
				if (t.IsAssignableFrom(current.GetType())) return current;
			}

			throw new Exception("No interaction in chain was of specified type");
		}

		public void Assume (FileSystemInfo info)
		{
			this["rootfilesystem"] = rootFilesystem;
			this["name"] = name = info.Name;

			string url;

			if (originalRequest.Length > 0)
				url = string.Format("/{0}/{1}", originalRequest, name);
			else
				url = "/" + name;

			this["url"] = url; 
			this["fullpath"] = fullPath =  info.FullName;

			FileInfo fileInfo = info as FileInfo;

			if (fileInfo != null)
				this["filesize"] = Filesize(fileInfo.Length);
			else 
				this["filesize"] = "-";

		}

		static string[] suffices = new string[] {
			"B", "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"
		};

		public static string Filesize (long count)
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

