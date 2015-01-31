using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.Extensions.BasicWeblings
{
	/// <summary>
	/// Sort of a subroutine substitute. You should probably not use this.
	/// Still there for legacy reasons.
	/// </summary>
	[Obsolete("Goes against design philosophy", false)]
	public class Relay : Service
	{
		private Settings modSettings { get; set; }
		private string RelayName { get; set; }

		public override string Description {
			get {
				return string.Format ("relay:{0}", RelayName);
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

			return Get(this.RelayName).TryProcess (interaction);
		}
			

		public RelayEntry Get(string name) 
		{
			return (RelayEntry)this.Root.Tags [string.Format ("relay.{0}", name)];
		}
	}
}
