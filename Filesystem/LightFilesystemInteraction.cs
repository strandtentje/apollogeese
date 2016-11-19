using System;
using System.IO;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;
using System.Threading;
using System.Collections.Generic;
using BorrehSoft.Utilities.Collections;
using BorrehSoft.Utilities.Log;

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

