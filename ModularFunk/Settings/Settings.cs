using System;
using System.Collections.Generic;
using System.IO;
using BorrehSoft.Utensils.Parsing;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.Utensils.Settings
{
	/// <summary>
	/// Settings data structure; stores objects by key. May be
	/// parsed from a file using the <see cref="BorrehSoft.Utensils.Settings.SettingsParser"/>
	/// </summary>
	public class Settings
	{
		Dictionary<string, object> assignments = 
			new Dictionary<string, object>();

		/// <summary>
		/// Gets the setting count on this level
		/// </summary>
		/// <value>The count.</value>
		public int Count {
			get { return assignments.Count; }
		}

		/// <summary>
		/// Gets or sets the <see cref="BorrehSoft.Utensils.Settings.Settings"/> with the specified indexer.
		/// </summary>
		/// <param name="indexer">Indexer.</param>
		public object this [string indexer] {
			get {
				if (assignments.ContainsKey(indexer)) return assignments[indexer];
				else return null;
			}
			set {
				if (assignments.ContainsKey(indexer)) assignments.Remove(indexer);
				assignments.Add(indexer, value);
			}
		}

		/// <summary>
		/// Gets the keys.
		/// </summary>
		/// <returns>The keys.</returns>
		public IEnumerable<string> GetKeys()
		{
			return assignments.Keys;
		}

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

			ParsingSession session = ParsingSession.FromFile(file);
			SettingsParser parser = new SettingsParser();
			object result;

			if (parser.Run (session, out result) < 0)
				return new Settings ();

			Settings config = (Settings)result;

			Secretary.Report (5, "Settings finished loading from: ", file, ", ", config.Count.ToString(), " root entries.");

			return config;
		}
	}
}

