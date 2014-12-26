using System;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class RelayExit : Service
	{
		private string BranchName { get; set; }
		private string RelayName { get; set; }
		private Service defaultBranch = Stub;

		public override string Description {
			get {
				return "Exit point for relayed flow";
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "default") 
				defaultBranch = e.NewValue;
		}

		protected override void Initialize (Settings modSettings)
		{
			BranchName = (string)modSettings ["branchname"];
			RelayName = (string)modSettings ["relayname"];
		}

		protected override bool Process (IInteraction parameters)
		{
			Service relayEntryExitBranch = defaultBranch;
			IInteraction sourceInteraction;

			if (parameters.TryGetClosest (typeof(RelayInteraction), out sourceInteraction)) {
				RelayInteraction relaySourceInteraction = (RelayInteraction)sourceInteraction;
				if (relaySourceInteraction.RelayName == RelayName) {
					object possibleRelayEntryExitBranch;
					if (relaySourceInteraction.Branches.TryGetValue (BranchName, out possibleRelayEntryExitBranch)) {
						relayEntryExitBranch = (Service)possibleRelayEntryExitBranch;
					}
				}
			}

			return relayEntryExitBranch.TryProcess (parameters);
		} 
	}
}

