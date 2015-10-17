using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.IO;
using System.Collections.Generic;
using BorrehSoft.Utensils.Log;

namespace InputProcessing
{
	public abstract class InputListing : TwoBranchedService
	{
		public override string Description {
			get {
				return "Input hub";
			}
		}

		[Instruction("Order of fields")]
		public IEnumerable<string> FieldOrder { get; set; }

		[Instruction("Show form when it was filled incorrectly", true)]
		public bool NegativeFeedback { get; set; }

		[Instruction("Show form when it was filled correctly", false)]
		public bool PositiveFeedback { get; set; }

		[Instruction("Fail the form reading session if unknown fields were tossed in", false)]
		public bool TollerateUnknownFields { get; set; }


		protected override void Initialize (Settings settings)
		{
			this.FieldOrder = settings.GetStringList ("fieldorder");
			this.NegativeFeedback = settings.GetBool ("negativefeedback", true);
			this.PositiveFeedback = settings.GetBool ("positivefeedback", false);
		}

		protected abstract IIncomingKeyValueInteraction GetReader (IInteraction parameters);

		bool ValidateInput (IIncomingKeyValueInteraction kvParameters, string inputName)
		{			
			if (Branches.Has (inputName)) {
				return Branches [inputName].TryProcess (kvParameters);
			} else {
				Secretary.Report (3, "No handler for field", inputName);
				return TollerateUnknownFields;
			}
		}

		bool ValidateInput(IIncomingKeyValueInteraction kvParameters) {			
			bool isValidationSuccessful = true;

			List<string> remainingFields = new List<string> (FieldOrder);

			kvParameters.Readable = true;

			while (kvParameters.ReadName()) {
				string inputName = kvParameters.GetName();
				if (remainingFields.Remove (inputName)) {
					isValidationSuccessful &= ValidateInput (kvParameters, inputName);
				} else {
					Secretary.Report (3, "Field literally not in order:", inputName);
				}
			}

			kvParameters.Readable = false;

			foreach (string fieldName in remainingFields) {
				isValidationSuccessful &= ValidateInput (kvParameters, fieldName);
			}

			return isValidationSuccessful;
		}

		protected override bool Process (IInteraction parameters)
		{
			IIncomingKeyValueInteraction kvParameters = GetReader (parameters);

			bool isValid = ValidateInput (kvParameters);

			bool isSuccessful = true;

			if (isValid) {
				isSuccessful &= Successful.TryProcess (kvParameters);
			} else {
				isSuccessful &= Failure.TryProcess (kvParameters);
			}

			if ((NegativeFeedback != isValid) || (PositiveFeedback == isValid)) {
				foreach(string orderName in FieldOrder) {
					Service feedback = kvParameters.Actions.Get (
						orderName, Branches [orderName]);
					isSuccessful &= feedback.TryProcess (kvParameters);
				}
			}

			return isSuccessful;
		}
	}
}

