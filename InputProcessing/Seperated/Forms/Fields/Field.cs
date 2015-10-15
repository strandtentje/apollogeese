using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace InputProcessing
{
	public abstract class Field<T> : TwoBranchedService
	{
		public override string Description {
			get {
				return string.Format ("{0} {1}-field", 
					(IsRequired ? "required" : "optional"),
					typeof(T).ToString ());
			}
		}

		[Instruction("Default value for this field")]
		public T Default {
			get;
			set;
		}

		[Instruction("Is this field required?")]
		public bool IsRequired {
			get;
			set;
		}

		protected override void Initialize (Settings settings)
		{
			this.IsRequired = settings.GetBool ("required", false);
		}

		private abstract Service FindActionForValue (object valueCandidate, out T value);

		protected override bool Process (IInteraction parameters)
		{
			bool successful = true;
			IInteraction formInteractionCandidate;

			if (parameters.TryGetClosest (
				typeof(IIncomingKeyValueInteraction), 
				out formInteractionCandidate)
			) {
				IIncomingKeyValueInteraction formInteraction = 
					(IIncomingKeyValueInteraction)formInteractionCandidate;

				T value;
				Service action = FindActionForValue (
					formInteraction.ReadValue (), 
					out value);

				formInteraction.SetCurrentValue (value);
				formInteraction.SetCurrentAction (action);

				successful = action == this.Successful;
			}

			return successful;
		}
	}

}

