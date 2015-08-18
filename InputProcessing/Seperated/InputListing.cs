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

		[Instruction("Always show form", false)]
		public bool AlwaysShowForm { get; set; }

		[Instruction("Fail the form reading session if unknown fields were tossen in", false)]
		public bool TollerateUnknownFields { get; set; }


		protected override void Initialize (Settings settings)
		{
			this.FieldOrder = settings.GetStringList ("fieldorder");
			this.AlwaysShowForm = settings.GetBool ("alwaysshowform", false);
			this.TollerateUnknownFields = settings.GetBool ("tollerateunknownfields", true);
		}

		protected abstract IIncomingKeyValueInteraction GetReader (IInteraction parameters);

		protected override bool Process (IInteraction parameters)
		{
			bool isValidationSuccessful = true;

			IIncomingKeyValueInteraction kvParameters = GetReader (parameters);

			while (kvParameters.ReadNextName()) {
				string inputName = kvParameters.GetCurrentName();
				if (Branches.Has (inputName)) {
					isValidationSuccessful &= Branches [inputName].TryProcess (kvParameters);
				} else {
					Secretary.Report (3, "Input contained field named", 
						inputName, "- we don't have a validator for that.");
					isValidationSuccessful &= TollerateUnknownFields;
				}
			}

			kvParameters.Finalized = true;

			bool isSuccessful = true;

			if (isValidationSuccessful) {
				isSuccessful &= Successful.TryProcess (kvParameters);
			} else {
				isSuccessful &= Failure.TryProcess (kvParameters);
			}

			if (AlwaysShowForm || !isValidationSuccessful) {
				foreach(string orderName in FieldOrder) {
					if (Branches.Has(orderName)) {
						isSuccessful &= Branches[orderName].TryProcess(kvParameters);
					} else {
						throw new MissingBranchException(orderName);
					}
				}
			}

			return isSuccessful;
		}
	}
}

