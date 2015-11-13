using System;
using System.IO;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Threading;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.ApolloGeese.Extensions.Filesystem
{
	public class LightFilesystemInteraction : SimpleInteraction 
	{
		public LightFilesystemInteraction(FileSystemInfo info, string[] keywords, string rootPath = "") {
			this["fullname"] = info.FullName;
			this["name"] = info.Name;
			this["url"] = info.FullName.Remove (0, rootPath.Length);
			this["keywords"] = keywords;
		}
	}
}

