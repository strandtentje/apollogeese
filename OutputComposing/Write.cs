using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Http;
using System.IO;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	public class Write : Service
	{
		public override string Description {
			get {
				return Format;
			}
		}

		private string VariableName { get; set; }
		private string Format { get; set; }

		protected override void Initialize (Settings modSettings)
		{
			VariableName = modSettings.GetString("variablename", "");

			if (modSettings.Has ("default")) {
				Format = modSettings.GetString ("default");
			} else {
				Format = modSettings.GetString ("format", "{0}");
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			IOutgoingBodiedInteraction interaction = (IOutgoingBodiedInteraction)parameters.GetClosest (typeof(IOutgoingBodiedInteraction));

			bool success = true;
			string text; 

			StreamWriter writer = interaction.GetOutgoingBodyWriter ();

			if (VariableName.Length == 0) {
				writer.WriteLine(Format);
			} else if (success = parameters.TryGetString (VariableName, out text)) {
				writer.WriteLine (string.Format(Format, text));
			}

			writer.Flush ();

			return success;
		}
	}
}

