using System;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Collections;

namespace ExternalData
{
	public class WwwForm : NameValueService
	{
		public override string Description {
			get {
				return "URL Encoded data parser";
			}
		}

		List<string> FieldList;

		bool Immediate;

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);
			this.FieldList = settings.GetStringList ("fieldlist");
			this.Immediate = settings.GetBool ("immediate", false);

			if (this.FieldList.Count == 0) {
				Secretary.Report (5, "Fieldlist Empty line:", this.ConfigLine.ToString());
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			TextReader reader;
			bool success;

			if (success = TryGetDatareader (parameters, null, out reader)) {
				// #yolo
				string fullData = reader.ReadToEnd ();

				string[] pairs = fullData.Split ('&');

				SimpleInteraction inputs = new SimpleInteraction (parameters);
				Map<IInteraction> interactions = new Map<IInteraction> ();

				foreach (string pair in pairs) {
					WwwInputInteraction input = new WwwInputInteraction (pair, parameters);

					if (FieldList.Contains (input.Name)) {
						if (Immediate)
							success &= TryReportPair (inputs, input);
						else {
							if (this.DoMapping) inputs [input.Name] = input.Value;
							interactions [input.Name] = input;
						}
					}
				}

				if (!Immediate) {
					foreach (string fieldName in this.FieldList) {
						IInteraction currentField;
						if (interactions.Has (fieldName))
							currentField = interactions [fieldName];
						else
							currentField = new SimpleInteraction(
								parameters, "name", fieldName);

						if (Branches.Has (fieldName))
							success &= Branches [fieldName].TryProcess (currentField);
						if (DoIterate)
							success &= this.Iterator.TryProcess (currentField);
					}
				}

				if (this.DoMapping)
					success &= this.Mapped.TryProcess (inputs);

			}

			return success;
		}
	}
}

