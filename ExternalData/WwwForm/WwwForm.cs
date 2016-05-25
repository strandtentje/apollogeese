using System;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Parsing;
using System.Web;

namespace ExternalData
{
	public class WwwForm : HttpForm<string>
	{
		public override string Description {
			get {
				return "URL Encoded data parser";
			}
		}

		bool Immediate;

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);
			this.Immediate = settings.GetBool ("immediate", false); 
		}

		private const char Concatenator = '&';
		private const char Assigner = '=';

		protected override void UrlParseReader (TextReader reader, NameValuePiper<TextReader, string>.NameValueCallback callback)
		{
			char currentCharacter;

			StringBuilder nameBuilder = new StringBuilder ();
			StringBuilder valueBuilder = new StringBuilder ();
			StringBuilder currentBuilder = nameBuilder;

			while (reader.Peek() > -1) {
				currentCharacter = (char)reader.Read ();

				switch (currentCharacter) {
				case Concatenator:						
					callback (nameBuilder.ToString (), HttpUtility.UrlDecode (valueBuilder.ToString ()));
					nameBuilder.Clear ();
					valueBuilder.Clear ();
					currentBuilder = nameBuilder;
					break;
				case Assigner:
					currentBuilder = valueBuilder;
					break;
				default:
					currentBuilder.Append (currentCharacter);
					break;
				}
			}

			if (nameBuilder.Length > 0) {
				if (valueBuilder.Length > 0) {
					callback (nameBuilder.ToString (), HttpUtility.UrlDecode (valueBuilder.ToString ()));
				} else {
					callback (nameBuilder.ToString (), "");
				}
			}
		}

		public override bool CheckMimetype (string mimeType)
		{
			return mimeType == "application/x-www-form-urlencoded";
		}

		protected override bool Process (IInteraction parameters)
		{
			TextReader urlDataReader = null;
			bool success = true;
			SimpleInteraction valuesByName = new SimpleInteraction (parameters);
			Map<IInteraction> inputInteractionsByName = new Map<IInteraction> ();

			success = success && TryGetDatareader (parameters, null, out urlDataReader);
			success = success && this.ParserRunner.TryRun (urlDataReader, delegate(string name, string value) {
				WwwInputInteraction inputInteractions = new WwwInputInteraction (name, value, parameters);

				if (StringFieldWhiteList.Contains (inputInteractions.Name)) {
					if (Immediate)
						success &= TryReportPair (valuesByName, inputInteractions);
					else {
						if (this.DoMapping)
							valuesByName [inputInteractions.Name] = inputInteractions.Value;
						inputInteractionsByName [inputInteractions.Name] = inputInteractions;
					}
				}
			});

			foreach (string fieldName in this.StringFieldWhiteList) {
				IInteraction currentField;

				if (inputInteractionsByName.Has (fieldName))
					currentField = inputInteractionsByName [fieldName];
				else
					currentField = new SimpleInteraction (parameters, "name", fieldName);

				success = success && (!Branches.Has (fieldName) || Branches [fieldName].TryProcess (currentField));
				success = success && (!DoIterate || this.Iterator.TryProcess (currentField));
			}			

			success = success && (!this.DoMapping || this.Mapped.TryProcess (valuesByName));
	
			return success;
		}
	}
}
