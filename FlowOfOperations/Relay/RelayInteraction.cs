using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.Extensions.BasicWeblings
{
	/// <summary>
	/// Interaction to do with Relays. Still don't use this.
	/// </summary>
	[Obsolete("Goes against design philosophy", false)]
	public class RelayInteraction : QuickInteraction
	{
		public string RelayName { get; private set; }
		public Map<Service> Branches { get; private set; }

		public RelayInteraction(IInteraction parent, Map<Service> branches, Settings modSettings, string relayName) : base(parent, modSettings)
		{
			this.RelayName = relayName;
			this.Branches = branches;
		}
	}
}

