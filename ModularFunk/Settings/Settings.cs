using System;
using System.Collections.Generic;
using System.IO;
using ModularFunk.Parsing;

namespace ModularFunk.Settings
{
	public class Settings
	{
		Dictionary<string, object> assignments = 
			new Dictionary<string, object>();

		public int Count {
			get { return assignments.Count; }
		}

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

		public static Settings FromFile(string file)
		{
			ParsingSession session = ParsingSession.FromFile(file);
			SettingsParser parser = new SettingsParser();
			Settings target;

			target = (Settings)session.Get(parser);

			return target;
		}
	}
}

