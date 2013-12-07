using System;
using System.Collections.Generic;
using System.IO;

namespace ModularFunk
{
	public class Settings
	{
		Dictionary<string, object> assignments = 
			new Dictionary<string, object>();


		public static Settings FromFile(string file)
		{
			Settings loadingSettings = new Settings();
			int position = 0;
			loadingSettings.TryParse(File.ReadAllText(file), ref position);
			return loadingSettings;
		}
	}
}

