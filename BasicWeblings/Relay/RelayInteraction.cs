using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class RelayInteraction : QuickInteraction
	{
		public Map<Service> Branches { get; private set; }

		public RelayInteraction(IInteraction parent, Map<Service> branches, Settings modSettings) : base(parent, modSettings)
		{
			this.Branches = branches;
		}
	}
}

