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

		[Instruction("Fail the form reading session if unknown fields were tossed in", false)]
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

			List<string> unprocessedFields = new List<string> (FieldOrder);

			while (kvParameters.ReadName()) {
				string inputName = kvParameters.GetName();
				if (Branches.Has (inputName) && unprocessedFields.Remove(inputName)) {
					isValidationSuccessful &= Branches [inputName].TryProcess (kvParameters);
				} else {
					Secretary.Report (3, "Unknown field name provided", inputName);
					isValidationSuccessful &= TollerateUnknownFields;
				}
			}

			foreach (string fieldName in unprocessedFields) {
				if (Branches.Has (fieldName)) {
					kvParameters.Actions [fieldName] = Branches [fieldName];
				} else {
					Secretary.Report (3, "Unknown field name provided", fieldName);
				}
			}

			bool isSuccessful = true;

			if (isValidationSuccessful) {
				isSuccessful &= Successful.TryProcess (kvParameters);
			} else {
				isSuccessful &= Failure.TryProcess (kvParameters);
			}

			if (AlwaysShowForm || !isValidationSuccessful) {
				foreach(string orderName in FieldOrder) {
					if (Branches.Has(orderName)) {
						Service action = kvParameters.Actions.Get (orderName, Branches [orderName]);
						isSuccessful &= action.TryProcess (kvParameters);
					}
				}
			}

			return isSuccessful;
		}
	}
}

