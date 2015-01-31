using System;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.Extensions.BasicWeblings
{	
	/// <summary>
	/// Named exit point for switching back to original branch. Don't use this, punk.
	/// </summary>
	[Obsolete("Goes against design philosophy", false)]
	public class RelayExit : Service
	{
		private string BranchName { get; set; }
		private string RelayName { get; set; }
		private Service defaultBranch = Stub;

		public override string Description {
			get {
				return string.Format ("relayexit:{0}:{1}", RelayName, BranchName);
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

			if (BranchName == null)
				throw new ConfigurationException ("branchname");
			if (RelayName == null)
				throw new ConfigurationException ("relayname");
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

