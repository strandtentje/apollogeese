using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace InputProcessing
{
	public abstract class ValueField : TwoBranchedService
	{
		public override string Description {
			get {
				throw new NotImplementedException ();
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{

		}

		private Service View { get;	set; }

		[Instruction("Use minimal limit when set to true", false)]
		public bool UseMin { get; private set; }

		[Instruction("Use maximal limit when set to true", false)]
		public bool UseMax { get; private set; }

		protected override void Initialize (Settings settings)
		{
			this.UseMin = settings.GetBool ("usemin", false);
			this.UseMax = settings.GetBool ("usemax", false);
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			base.HandleBranchChanged (sender, e);

			if (e.Name == "view")
				this.View = e.NewValue;
		}
			
		protected override bool Process (IInteraction parameters)
		{
			bool successful = true;
			IInteraction formInteractionCandidate;



			if (parameters.TryGetClosest (typeof(IIncomingKeyValueInteraction), out formInteractionCandidate)) {
				IIncomingKeyValueInteraction formInteraction = (IIncomingKeyValueInteraction)formInteractionCandidate;

				if (formInteraction.IsViewing) {
					successful = this.View.TryProcess (parameters);
				} else {
					object valueCandidate;
					/* 
					if (TryReadValue (formInteraction, out valueCandidate)) {

					}
					formInteraction.SetCurrentValue (ReadValue ());*/
				}
			}

			return false;
		}
	}
}

