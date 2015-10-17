using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.IO;
using System.Collections.Generic;
using BorrehSoft.Utensils.Log;

namespace InputProcessing
{
	/// <summary>
	/// Lists inputs; fires up validators.
	/// </summary>
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

		/// <summary>
		/// Gets the reader for input data.
		/// </summary>
		/// <returns>The reader.</returns>
		/// <param name="parameters">Parameters.</param>
		protected abstract IIncomingKeyValueInteraction GetReader (IInteraction parameters);

		/// <summary>
		/// Validates single input by name
		/// </summary>
		/// <returns><c>true</c>, if input was validated, <c>false</c> otherwise.</returns>
		/// <param name="kvParameters">Kv parameters.</param>
		/// <param name="inputName">Input name.</param>
		bool ValidateInput (IIncomingKeyValueInteraction rawInput, string inputName)
		{			
			if (Branches.Has (inputName)) {
				return Branches [inputName].TryProcess (rawInput);
			} else {
				Secretary.Report (3, "No handler for field", inputName);
				return TollerateUnknownFields;
			}
		}

		/// <summary>
		/// Validates multiple inputs by kv-pairs
		/// </summary>
		/// <returns><c>true</c>, if input was valid, <c>false</c> otherwise.</returns>
		/// <param name="kvParameters">Kv parameters.</param>
		bool ValidateInput(IIncomingKeyValueInteraction rawInput) {			
			bool isValidationSuccessful = true;

			List<string> remainingFields = new List<string> (FieldOrder);

			rawInput.IsValueAvailable = true;

			while (rawInput.ReadName()) {
				string inputName = rawInput.GetName();
				if (remainingFields.Remove (inputName)) {
					isValidationSuccessful &= ValidateInput (rawInput, inputName);
				} else {
					Secretary.Report (3, "Field literally not in order:", inputName);
				}
			}

			rawInput.IsValueAvailable = false;

			foreach (string fieldName in remainingFields) {
				isValidationSuccessful &= ValidateInput (rawInput, fieldName);
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

			if ((NegativeFeedback == !isValid) || (PositiveFeedback == isValid)) {
				foreach(string orderName in FieldOrder) {
					Service feedback = kvParameters.Feedback.Get (orderName, Branches [orderName]);
					isSuccessful &= feedback.TryProcess (kvParameters);
				}
			}

			return isSuccessful;
		}
	}
}
