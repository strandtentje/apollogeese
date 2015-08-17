using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.IO;
using System.Collections.Generic;

namespace InputProcessing
{
	public abstract class Form : TwoBranchedService
	{
		public override string Description {
			get {
				return "Input hub";
			}
		}

		protected override void Initialize (Settings settings)
		{

		}

		protected abstract bool TryGetNameValue (IInteraction parameters, out KeyValuePair<string, Stream> result) {

		}

		private bool BranchForCandidateInput (KeyValuePair<string, TextReader> candidateInput, IInteraction parent)
		{
			bool candidateSuccessful = false;

			if (Branches.Has (candidateInput.Key)) {
				FieldInteraction fieldPayload;
				Service fieldProcessor;

				fieldPayload = new FieldInteraction (candidateInput.Value, parent);
				fieldProcessor = Branches [candidateInput.Key];

				if (fieldProcessor.TryProcess (fieldPayload)) {
					candidateSuccessful = true;
				}
			}

			return candidateSuccessful;
		}

		protected override bool Process (IInteraction parameters)
		{
			bool isSuccessful = true;
			bool candidateSuccessful;
			KeyValuePair<string, TextReader> candidateInput;

			while (TryGetNameValue (parameters, out candidateInput)) {
				isSuccessful &= BranchForCandidateInput (candidateInput, parameters);
			}
		}
	}
}

