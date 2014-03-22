using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Duckling.HTML.Entities;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page.DataEntry.Fields
{
	public class SubmitButton : Service
	{
		BodylessEntity ButtonTag = new BodylessEntity("input");

		public override string Description {
			get {
				return "A mere submit-button";
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			ButtonTag.Attributes ["type"] = "submit";
			ButtonTag.Attributes ["value"] = modSettings.GetString ("text", "submit");
		}

		protected override bool Process (IInteraction parameters)
		{
			(parameters as EntryInteraction).FormDisplaying += HandleFormDisplaying;

			return true;
		}

		void HandleFormDisplaying (object sender, FormDisplayingEventArgs e)
		{
			ButtonTag.WriteWithDelegate(e.Writer.Write);
		}
	}
}

