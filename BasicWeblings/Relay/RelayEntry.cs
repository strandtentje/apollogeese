using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class RelayEntry : Service
	{
		private string RelayName { get; set; }
		private Service Begin { get; set; }

		public override string Description {
			get {
				return string.Format ("relayentry:{0}", this.RelayName);
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "begin")
				this.Begin = e.NewValue;
		}

		protected override void Initialize (Settings modSettings)
		{
			this.RelayName = (string)modSettings ["relayname"];
			this.Root.Tags [string.Format ("relay.{0}", this.RelayName)] = this;
		}

		protected override bool Process (IInteraction parameters)
		{
			return Begin.TryProcess (parameters);
		}
	}
}

