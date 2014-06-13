using System;
using System.Collections.Generic;
using System.IO;
using BorrehSoft.Utensils.Parsing;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.Utensils.Collections.Settings
{
	/// <summary>
	/// Settings data structure; stores objects by key. May be
	/// parsed from a file using the <see cref="BorrehSoft.Utensils.Settings.SettingsParser"/>
	/// </summary>
	public class Settings : Map<object>
	{
		/// <summary>
		/// Acquires settings from the file.
		/// </summary>
		/// <returns>The file.</returns>
		/// <param name="file">File.</param>
		public static Settings FromFile(string file)
		{
			Secretary.Report (5, "Loading settings file ", file);

			if (!File.Exists (file)) {
				File.Create (file);
				Secretary.Report (5, file, " didn't exist. Has been created.");
			}

			ParsingSession session = ParsingSession.FromFile(file, new IncludeParser());
			SettingsParser parser = new SettingsParser();
			object result;

			if (parser.Run (session, out result) < 0)
				return new Settings ();

			Settings config = (Settings)result;

			Secretary.Report (5, "Settings finished loading from: ", file);

			return config;
		}

		public bool GetBool (string id, bool otherwise)
		{
			if (base.Has(id))
				return (bool)base[id];

			return otherwise;
		}

	}
}

