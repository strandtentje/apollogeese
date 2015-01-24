using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class JumpInteraction : QuickInteraction
	{
		public Map<Service> Branches { get; private set; }

		public JumpInteraction(IInteraction parent, Map<Service> branches, Settings modSettings) : base(parent, modSettings)
		{
			this.Branches = branches;
		}
	}
}

