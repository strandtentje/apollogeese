using System;
using System.Text.RegularExpressions;
using System.Web;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils;
using BorrehSoft.Utensils.Settings;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page.DataEntry.Fields
{
	public class TextEntry : Service
	{
		Regex InputMatcher;
		HtmlTag InputTag = new HtmlTag ("input", DontClose: true);
		HtmlTag FaultyInputTag = new HtmlTag ("input", DontClose: true);
		HtmlTag LabelTag = new HtmlTag ("label");

		string labelBase, entireLabel, fieldName;

		public override string[] AdvertisedBranches {
			get {
				return new string[] { };
			}
		}

		public override string Description {
			get {
				return "Text Entry Component";
			}
		}


		protected override void Initialize (Settings modSettings)
		{		
			InputMatcher = new Regex (modSettings.GetString ("regexconstraint", ".*"));

			InputTag.Attributes ["type"] = FaultyInputTag.Attributes ["type"] = 
					modSettings.GetString ("type", "text");

			if (modSettings.TryGetString ("name", out fieldName)) 
				InputTag.Attributes ["name"] = 
					FaultyInputTag.Attributes ["name"] = 
					LabelTag.Attributes ["for"] = 
					fieldName;
									
			if (!modSettings.TryGetString ("label", out labelBase)) labelBase = "";

			InputTag.Attributes ["value"] = FaultyInputTag.Attributes ["value"] = "{0}";

			InputTag.Rerender (); FaultyInputTag.Rerender (); LabelTag.Rerender ();

			entireLabel = LabelTag.Head + labelBase + LabelTag.Tail;

		}

		protected override bool Process (IInteraction parameters)
		{
			EntryInteraction interaction = parameters as EntryInteraction;
			
			if (interaction == null)
				return false;

			interaction.FormDisplaying	+= HandleFormDisplaying;
			interaction.InputAccepted += HandleInputAccepted;

			bool success = VerifyInput (interaction.Values [fieldName] ?? "");
			return success;
		}

		/// <summary>
		/// Verifies if specified string is a valid input for this box
		/// </summary>
		/// <returns><c>true</c>, if input was verified as correct, <c>false</c> otherwise.</returns>
		/// <param name="str">String to verify.</param>
		bool VerifyInput (string str)
		{
			return InputMatcher.IsMatch (str);
		}

		/// <summary>
		/// Acts in the event the form has to be displayed (again)
		/// This gets by means of invocation of an event of an earlier
		/// parameter-set.
		/// </summary>
		/// <param name="sender">Sender Interaction</param>
		/// <param name="e">Arguments relevant to displaying the form</param>
		void HandleFormDisplaying (object sender, FormDisplayingEventArgs e)
		{
			e.Writer.Write (entireLabel);

			string input = e.Values [fieldName] ?? "";
			HtmlTag appropriateTag;

			appropriateTag = (e.EntryAttempt && !VerifyInput (input)) ? FaultyInputTag : InputTag;

			e.Writer.Write (appropriateTag.Head, HttpUtility.HtmlAttributeEncode (input));
			e.Writer.Write (appropriateTag.Tail);
		}

		/// <summary>
		/// Acts in the event the form has been accepted and the values
		/// have to be put into the main interaction
		/// </summary>
		/// <param name="sender">Sender interaction</param>
		/// <param name="e">Arguments relevant to processing the entries</param>
		void HandleInputAccepted (object sender, InputAcceptedEventArgs e)
		{
			e.Parameters[fieldName] = e.Values [fieldName] ?? "";
		}
	}
}

