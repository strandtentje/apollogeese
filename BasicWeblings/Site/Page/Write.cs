using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Duckling.Http;

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

			if (VariableName.Length == 0) {
				interaction.OutgoingBody.WriteLine(Format);
			} else if (success = parameters.TryGetString (VariableName, out text)) {
				interaction.OutgoingBody.WriteLine (string.Format(Format, text));
			}

			return success;
		}
	}
}

