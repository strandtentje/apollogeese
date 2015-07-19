using System;
using System.Collections.Generic;
using System.IO;
using BorrehSoft.Utensils.Parsing;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Parsing.Parsers;

namespace BorrehSoft.Utensils.Collections.Settings
{
	/// <summary>
	/// Settings data structure; stores objects by key. May be
	/// parsed from a file using the <see cref="BorrehSoft.Utensils.Settings.SettingsParser"/>
	/// </summary>
	public class Settings : Map<object>
	{
		public Settings() {}

		public Settings(Map<object> origin) : base(origin){}

		new public Settings Clone() 
		{
			return new Settings (base.Clone());
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

			ParsingSession session = ParsingSession.FromFile(file, new IncludeParser());
			Directory.SetCurrentDirectory (session.SourceFile.Directory.FullName);
			SettingsParser parser = new SettingsParser();
			object result;

			Settings config;

			if (parser.Run (session, out result) < 0)
				config = new Settings ();
			else 
				config = (Settings)result;

			config.SourceFile = session.SourceFile;

			Secretary.Report (5, "Settings finished loading from: ", file);

			// Secretary.Report (6, session.ParsingProfiler.FinalizeIntoReport().ToString());

			return config;
		}

		public FileInfo SourceFile { get; private set; }

		public static Settings FromJson (string data)
		{
			ParsingSession session = new ParsingSession(data, new WhitespaceParser());
			SettingsParser parser = new SettingsParser(entitySe: ',', couplerChar: ':');

			object result;

			if (parser.Run(session, out result) < 0)
				return new Settings();

			return (Settings) result;
		}

		public bool GetBool(string id) {
			if (Has (id)) {
				object boolObj = Get (id);
				if (boolObj is bool) {
					return (bool)boolObj;
				}
			}

			throw new MissingSettingException ("", id, "int");
		}

		public bool GetBool (string id, bool otherwise)
		{
			if (base.Has(id))
				return (bool)base[id];

			return otherwise;
		}		

		public Settings GetSubsettings (string name)
		{
			Settings subsettings = this[name] as Settings;

			// if (subsettings == null) Secretary.Report(8, "No subsettings at ", name);

			return subsettings ?? new Settings();
		}		

		public int GetInt(string id) {
			if (Has (id)) {
				object intObj = Get (id);
				if (intObj is int) {
					return (int)intObj;
				}
			}

			throw new MissingSettingException ("", id, "int");
		}

        public float GetFloat(string id)
        {
            if (Has(id))
            {
                object floatObj = Get(id);
                if (floatObj is float)
                {
                    return (float)floatObj;
                }
            }

            throw new MissingSettingException("", id, "float");
        }

		public IEnumerable<string> GetStringList(string id, params string[] defaults) {
			IEnumerable<object> list = (IEnumerable<object>)base[id];
			List<string> stringList = new List<string> ();

			foreach (object item in list) 
				stringList.Add ((string)item);

			return stringList;
		}

		public int GetInt(string id, int otherwise)
		{
			int result;
			object resultObject = base [id];

			if (resultObject != null) {
				if (resultObject is int) {
					return (int)resultObject;
				} else if (resultObject is string) {
					if (int.TryParse ((string)resultObject, out result)) {
						return result;
					}
				}
			}

			return otherwise;
		}

		public object Tag {
			get;
			set;
		}

    }
}

