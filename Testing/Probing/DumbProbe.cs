using System;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace Probing
{
	public class DumbProbe : SingleBranchService
	{
		public override string Description {
			get {
				return "Dumb probe";
			}
		}

		public long CallCounter { get; private set; }

		public IFast LastInteraction { get; private set; }

		public override bool FastProcess (IFast parameter)
		{
			CallCounter++;
			LastInteraction = parameter;
			if (parameter is IInteraction) {
				return WithBranch.TryProcess ((IInteraction)parameter);
			} else {
				return true;
			}
		}
	}
}

