using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;
using System.Net;
using BorrehSoft.Utensils;
using System.Collections.Generic;

namespace BorrehSoft.Extensions.FileServer
{
	class Mapping
	{
		public string URL { get; set; }
		public string FileSystem { get; set; }
		public Settings Whitelist { get; set; }
		public bool AllowBrowsing { get; set; }

		public Mapping (Settings s)
		{
			URL = (string)s ["url"];
			FileSystem = (string)s ["filesystem"];
			Settings = (Settings)s ["whitelist"];
			AllowBrowsing = (bool)s ["allowbrowsing"];
		}

		public bool Follow (string rawUrl, HttpListenerResponse response)
		{
			throw new NotImplementedException ();
		}
	}
}