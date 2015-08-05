using System;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.ApolloGeese.Duckling;

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

		public override Map<object> GetParseableInput (IInteraction parameters)
		{
			IInteraction incomingInteraction;
			Map<object> input;

			if (parameters.TryGetClosest (typeof(IIncomingBodiedInteraction), out incomingInteraction)) {
				IIncomingBodiedInteraction incomingBody = (IIncomingBodiedInteraction)incomingInteraction;

				Settings parsedData = SettingsParser.FromJson (incomingBody.GetIncomingBodyReader().ReadToEnd());
				Queue<string> pathQueue = new Queue<string> (readPath);

				while (pathQueue.Count > 0) parsedData = (Settings)parsedData[pathQueue.Dequeue()];

				input = parsedData;
			} else {
				throw new Exception ("No incoming body found to read from.");
			}

			return input;
		}
	}
}

