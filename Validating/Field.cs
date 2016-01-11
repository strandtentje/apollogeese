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
			this.VariableName = settings.GetString ("variable", "value");
		}

		protected abstract bool CheckValid (object rawInput);

		public string VariableName {
			get;
			set;
		}

		protected override bool Process (IInteraction parameters)
		{
			bool successful = true;
			IInteraction formInteractionCandidate;

			object candidateValue;

			if (parameters.TryGetFallback(VariableName, out candidateValue)) {
				if (CheckValid (candidateValue)) {
					successful &= Successful.TryProcess (parameters);
				} else {
					successful &= Failure.TryProcess (parameters);
				}
			} else if (IsRequired) {
				successful &= Failure.TryProcess (parameters);
			}

			return successful;
		}
	}

}

