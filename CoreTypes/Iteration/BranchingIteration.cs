using System;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace Iteration
{
	public class BranchingIteration
	{
		private int InteractionCount;
		private IInteraction LastInteraction;

		public IterationBranches Branches { get; private set; }

		public BranchingIteration (IterationBranches branches)
		{
			Branches = branches;
		}

		public bool Register (IInteraction interaction)
		{
			bool success = true;
			if ((InteractionCount == 1) && (Branches.First != null)) {
				success = Branches.First.TryProcess (LastInteraction);
			} else if (InteractionCount > 0) {
				success = Branches.Iterator.TryProcess (LastInteraction);
			}

			InteractionCount++;
			LastInteraction = interaction;

			return success;
		}

		public bool Finish (IInteraction fallBack)
		{
			if (InteractionCount == 0) {
				return Branches.None == null || Branches.None.TryProcess (fallBack);
			} else if ((InteractionCount == 1) && (Branches.Single != null)) {
				return Branches.Single.TryProcess (LastInteraction);
			} else if (Branches.Last != null) {
				return Branches.Last.TryProcess (LastInteraction);
			} else {
				return Branches.Iterator.TryProcess (LastInteraction);
			}
		}
	}
}

