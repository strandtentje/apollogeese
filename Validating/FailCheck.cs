using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace Validating
{
	public class FailCheck : Service
	{
		public override string Description {
			get {
				return "Finds nearest checkinteraction, shouts failure.";
			}
		}

		protected override void Initialize (Settings settings)
		{
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}

		protected override bool Process (IInteraction parameters)
		{
			IInteraction candidate;
			bool successful;

			if (successful = parameters.TryGetClosest (typeof(CheckInteraction), out candidate)) {
				CheckInteraction callback = (CheckInteraction)candidate;
				callback.Successful = false;
			}

			return successful;
		}
	}
}

