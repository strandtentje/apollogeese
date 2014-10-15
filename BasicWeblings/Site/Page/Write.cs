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
				return "Writer for single variable in interaction";
			}
		}

		private string VariableName { get; set; }

		protected override void Initialize (Settings modSettings)
		{
			VariableName = modSettings.GetString("variablename", "");
		}

		protected override bool Process (IInteraction parameters)
		{
			IHttpInteraction interaction = (IHttpInteraction)parameters.GetClosest (typeof(IHttpInteraction));

			bool success; string text; 

			if (success = parameters.TryGetString (VariableName, out text)) {
				interaction.ResponseBody.WriteLine (text);
			}

			return success;
		}
	}
}

