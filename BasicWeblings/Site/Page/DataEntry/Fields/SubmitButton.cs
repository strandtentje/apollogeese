using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page.DataEntry.Fields
{
	public class SubmitButton : Service
	{
		HtmlTag InputTag = new HtmlTag ("input", DontClose: true);

		public override string[] AdvertisedBranches {
			get {
				return new string[] { };
			}
		}

		public override string Description {
			get {
				return "A mere submit-button";
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			InputTag.Attributes ["type"] = "submit";
			InputTag.Attributes ["value"] = modSettings.GetString ("text", "submit");
			InputTag.Rerender ();
		}

		protected override bool Process (IInteraction parameters)
		{
			(parameters as EntryInteraction).FormDisplaying += HandleFormDisplaying;

			return true;
		}

		void HandleFormDisplaying (object sender, FormDisplayingEventArgs e)
		{
			e.Writer.Write (InputTag.Head);
			e.Writer.Write (InputTag.Tail);
		}
	}
}

