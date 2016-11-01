using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;

namespace Testing
{
	public class HaltRecursion : SingleBranchService
	{
		public override string Description {
			get {
				return "This service may not be crossed twice.";
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			IInteraction foundInteraction;

			if (parameters.TryGetClosest(typeof(RecursionMarkerInteraction), out foundInteraction) && ((RecursionMarkerInteraction)foundInteraction).Placer == this) {
				throw new Exception ("Recursion halted");
			} else {
				return WithBranch.TryProcess (new RecursionMarkerInteraction (parameters, this));
			}
		}
	}
}

