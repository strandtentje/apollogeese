using System;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Duckling;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class RelayExit : Service
	{
		private string BranchName { get; set; }

		public override string Description {
			get {
				return "Exit point for relayed flow";
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}

		protected override void Initialize (Settings modSettings)
		{
			BranchName = (string)modSettings ["branchname"];
		}

		protected override bool Process (IInteraction parameters)
		{
			RelayInteraction relaySourceInteraction;
			Service relayEntryExitBranch;

			relaySourceInteraction = (RelayInteraction)parameters.GetClosest (typeof(RelayInteraction));
			relayEntryExitBranch = relaySourceInteraction.Branches [BranchName];

			return relayEntryExitBranch.TryProcess (parameters);
		} 
	}
}

