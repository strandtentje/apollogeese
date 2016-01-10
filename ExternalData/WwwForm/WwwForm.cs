using System;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;

namespace ExternalData
{
	public class WwwForm : NameValueService
	{
		public override string Description {
			get {
				return "URL Encoded data parser";
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

				foreach (string pair in pairs) {
					WwwInputInteraction input = new WwwInputInteraction (pair, parameters);

					success &= TryReportPart (inputs, input);
				}

				if (this.DoMapping)
					success &= this.Mapped.TryProcess (inputs);

			}

			return success;
		}
	}
}

