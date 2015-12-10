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

		public Service Empty { 
			get {
				return Branches.Get ("empty", this.Failure);
			}
		}

		protected override void Initialize (Settings settings)
		{
			this.FieldOrder = settings.GetStringList ("fieldorder");
		}

		/// <summary>
		/// Gets the reader for input data.
		/// </summary>
		/// <returns>The reader.</returns>
		/// <param name="parameters">Parameters.</param>
		protected abstract IRawInputInteraction GetReader (IInteraction parameters);

		/// <summary>
		/// Try to branch for unfamiliar input
		/// </summary>
		/// <returns><c>true</c>, if branch was tryed, <c>false</c> otherwise.</returns>
		/// <param name="branchName">Branch name.</param>
		/// <param name="rawInput">Raw input.</param>
		/// <param name="inputName">Input name.</param>
		/// <param name="successful">If set to <c>true</c> successful.</param>
		bool TryBranch (string branchName, IRawInputInteraction rawInput, string inputName, bool successful = true)
		{			
			if (Branches.Has (branchName)) {
				successful = Branches [branchName].TryProcess (
					new KeyValueInteraction (rawInput, inputName, 
				                          rawInput.ReadInput ()));
			} else {
				rawInput.SkipInput ();
				Secretary.Report (5, 
				                  "input", inputName, 
				                  "remained uncaught by", branchName, 
				                  "and was skipped consequently");
			}

			return successful;
		}

		/// <summary>
		/// Validates single input by name
		/// </summary>
		/// <returns><c>true</c>, if input was validated, <c>false</c> otherwise.</returns>
		/// <param name="kvParameters">Kv parameters.</param>
		/// <param name="inputName">Input name.</param>
		bool ValidateInput (IRawInputInteraction rawInput, string inputName)
		{			
			if (Branches.Has (inputName)) {
				return Branches [inputName].TryProcess (rawInput);
			} else {
				return TryBranch("iterator", rawInput, inputName);
			}
		}

		/// <summary>
		/// Validates multiple inputs by kv-pairs
		/// </summary>
		/// <returns><c>true</c>, if input was valid, <c>false</c> otherwise.</returns>
		/// <param name="kvParameters">Kv parameters.</param>
		bool ValidateInput(IRawInputInteraction rawInput) {			
			bool isValidationSuccessful = true;

			List<string> remainingFields = new List<string> (FieldOrder);

			rawInput.HasValuesAvailable = true;

			while (rawInput.ReadNextName()) {
				rawInput.InputCount++;
				string inputName = rawInput.CurrentName;
				if (remainingFields.Remove (inputName)) {
					isValidationSuccessful &= ValidateInput (rawInput, inputName);
				} else {
					isValidationSuccessful &= TryBranch ("catchall", rawInput, inputName);
				}
			}

			rawInput.HasValuesAvailable = false;

			foreach (string fieldName in remainingFields) {
				rawInput.CurrentName = fieldName;
				isValidationSuccessful &= ValidateInput (rawInput, fieldName);
			}

			return isValidationSuccessful;
		}

		/// <summary>
		/// Gets the conclusion.
		/// </summary>
		/// <returns>The conclusion.</returns>
		/// <param name="kvParameters">Kv parameters.</param>
		/// <param name="isValid">If set to <c>true</c> is valid.</param>
		/// <param name="showFeedback">Show feedback.</param>
		Service GetConclusion(IRawInputInteraction kvParameters, bool isValid) {
			Service conclusion = Empty;
						
			if (kvParameters.InputCount > 0) {
				if (isValid) {
					conclusion = Successful;
				} else {
					conclusion = Failure;
				}
			}

			return conclusion;
		}

		protected override bool Process (IInteraction parameters)
		{
			Service conclusion;

			try {
				IRawInputInteraction kvParameters = GetReader (parameters);

				bool isValid = ValidateInput (kvParameters);

				conclusion = GetConclusion (kvParameters, isValid);

				parameters = kvParameters;
			} catch(FormException ex) {
				Secretary.Report (6, 
					"User input rejected due to form exception:",
					ex.Message);

				conclusion = Empty;
			}

			return conclusion.TryProcess(parameters);
		}
	}
}
