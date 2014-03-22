using System;
using System.Text.RegularExpressions;
using System.Web;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Duckling.HTML.Entities;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page.DataEntry.Fields
{
	public class TextEntry : Service
	{
		private Regex inputMatcher;
		private BodylessEntity inputTag = new BodylessEntity(Name: "input");
		private BodylessEntity faultyInputTag = new BodylessEntity(Name: "input");
		private TextualEntity labelTag = new TextualEntity(Name: "input");	
		private string id, type, label;

		public override string Description {
			get {
				return "Text Entry Component";
			}
		}

		/// <summary>
		/// Gets or sets the name/for of this input.
		/// </summary>
		/// <value>
		/// The name/for
		/// </value>
		public string ID {
			get {
				return id;
			}
			set {
				id = value;

				inputTag.Attributes.Name = id;
				faultyInputTag.Attributes.Name = id;
				labelTag.Attributes.For = id;
			}
		}

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		public string Type {
			get {
				return type;
			}
			set {
				type = value;

				inputTag.Attributes.Type = value;
				faultyInputTag.Attributes.Type = value;
			}
		}

		/// <summary>
		/// Gets or sets the label.
		/// </summary>
		/// <value>
		/// The label.
		/// </value>
		public string Label {
			get {
				return label;
			}
			set {
				label = value;

				labelTag.Attributes.Value = label;
			}
		}

		protected override void Initialize (Settings modSettings)
		{		
			inputMatcher = new Regex (modSettings.GetString ("regexconstraint", ".*"));

			ID = modSettings["name"] as string ?? "missingno";
			Type = modSettings["type"] as string ?? "text";
			Label = modSettings["label"] as string ?? "Field";
		}

		protected override bool Process (IInteraction parameters)
		{
			bool success = false;
			EntryInteraction interaction = parameters as EntryInteraction;
			
			if (interaction != null) {
				interaction.FormDisplaying += HandleFormDisplaying;
				interaction.InputAccepted += HandleInputAccepted;

				success = VerifyInput (interaction.Values [fieldName] ?? "");
			}

			return success;
		}

		/// <summary>
		/// Verifies if specified string is a valid input for this box
		/// </summary>
		/// <returns><c>true</c>, if input was verified as correct, <c>false</c> otherwise.</returns>
		/// <param name="str">String to verify.</param>
		bool VerifyInput (string str)
		{
			return inputMatcher.IsMatch (str);
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
			labelTag.WriteUsingCallback(e.Writer.Write);


			string input = e.Values [fieldName];
			if (!e.EntryAttempt || VerifyInput (input)) {
				inputTag.WriteUsingCallback(e.Writer.Write);
			} else {
				FaultyInputTag.WriteUsingCallback(e.Writer.Write);
			}
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

