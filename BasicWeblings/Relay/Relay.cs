using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class Relay : Service
	{
		private Settings modSettings { get; set; }
		private string RelayName { get; set; }

		public override string Description {
			get {
				return "Relay to other tree";
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}

		protected override void Initialize (Settings modSettings)
		{
			this.RelayName = (string)modSettings ["relayname"];
			this.modSettings = modSettings;
		}

		protected override bool Process (IInteraction parameters)
		{
			RelayInteraction interaction = new RelayInteraction (parameters, this.Branches, this.modSettings, this.RelayName);

			return RelayEntry.Get(this.RelayName).TryProcess (interaction);
		}
	}
}

