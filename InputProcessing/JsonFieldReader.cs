using System;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.InputProcessing
{
	/// <summary>
	/// Reads fields from JSON-construction from incoming body into context
	/// </summary>
	public class JsonFieldReader : FieldReader
	{
		string[] readPath;

		protected override void Initialize (Settings modSettings)
		{
			base.Initialize (modSettings);

			readPath = modSettings.GetString("readpath", "").Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
		}

		public override Map<object> Deserialize (string data)
		{
			Settings parsedData = Settings.FromJson (data);
			Queue<string> pathQueue = new Queue<string> (readPath);

			while (pathQueue.Count > 0) parsedData = (Settings)parsedData[pathQueue.Dequeue()];

			return parsedData;
		}

		protected override string SourceName {
			get {
				return "jsonfields";
			}
		}
	}
}

