using System;
using BorrehSoft.ApolloGeese.Duckling;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class DirectedInteraction : QuickInteraction, IInteraction
	{
		public string BranchName { get; private set; }

		public DirectedInteraction (IInteraction parent, string branch) : base(parent)
		{
			this.BranchName = branch;
		}
	}
}

