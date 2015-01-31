using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Http;
using System.IO;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page
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
			Format = modSettings.GetString("format", "{0}");
		}

		protected override bool Process (IInteraction parameters)
		{
			IHttpInteraction interaction = (IHttpInteraction)parameters.GetClosest (typeof(IHttpInteraction));

			bool success = true;
			string text; 

			StreamWriter writer = interaction.GetOutgoingBodyWriter ();

			if (VariableName.Length == 0) {
				writer.WriteLine(Format);
			} else if (success = parameters.TryGetString (VariableName, out text)) {
				writer.WriteLine (string.Format(Format, text));
			}

			return success;
		}
	}
}

