using System;
using System.Collections;
using System.IO;
using Parsing;
using SimpleJson.Transcoder;
using System.Dynamic;
using BorrehSoft.Utilities.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace ExternalData
{
	public static class JsonFormulator
	{
		public static object Formulate (object value)
		{
			if (value is Hashtable) {
				// i'm so sorry
				return JsonSerializer.SerializeObject (value);
			} else if (value is ArrayList) {
				// i'm so incredibly sorry
				return JsonSerializer.SerializeObject (value);
			} else {
				return value;
			}
		}
	}

	public class JsonForm : BriefForm<object>
	{
		public override bool CheckMimetype (string mimeType)
		{
			return mimeType == "application/json";
		}

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);
		}

		public override string Description {
			get {
				return "Parse JSON";
			}
		}

		protected override void UrlParseReader (TextReader reader, NameValuePiper<TextReader, object>.NameValueCallback callback)
		{
			var jsonObject = JsonSerializer.DeserializeString (reader.ReadToEnd ()) as Hashtable;

			foreach (DictionaryEntry entry in jsonObject) {
				callback (entry.Key.ToString (), JsonFormulator.Formulate (entry.Value));
			}
		}
	}
}

