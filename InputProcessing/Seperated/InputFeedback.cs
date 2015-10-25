using System;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace InputProcessing
{
	public class InputFeedback : Service
	{
		public override string Description {
			get {
				throw new NotImplementedException ();
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			IInteraction uncastFeedback;
			bool success = parameters.TryGetClosest (typeof(IRawInputInteraction), out uncastFeedback);

			if (success) {
				IRawInputInteraction inputLog = (IRawInputInteraction)uncastFeedback;

				foreach (string orderName in inputLog.FieldOrder) {
					Service feedback = inputLog.Feedback.Get (orderName, Branches [orderName]);
					success &= feedback.TryProcess (parameters);
				}
			} 

			return success;
		}
	}
}

