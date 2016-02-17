using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace Validating
{
	public class CheckCheck : TwoBranchedService
	{
		public override string Description {
			get {
				return "Branch successful if Check-session was succesful" +
				" thus far";
			}
		}

		protected override void Initialize (Settings settings)
		{
			
		}

		protected override bool Process (IInteraction parameters)
		{
			IInteraction candidate;
			bool successful;

			if (successful = parameters.TryGetClosest (typeof(CheckInteraction), out candidate)) {
				CheckInteraction callback = (CheckInteraction)candidate;
				if (callback.Successful) {
					successful = this.Successful.TryProcess (parameters);
				} else {
					successful = this.Failure.TryProcess (parameters);
				}
			}

			return successful;
		}
	}
}

