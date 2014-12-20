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
				return "Entry point for relayed flow";
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
			RelayEntries.Add (RelayName, this);
		}

		protected override bool Process (IInteraction parameters)
		{
			return Begin.TryProcess (parameters);
		} 

		private static Dictionary<string, RelayEntry> RelayEntries = new Dictionary<string, RelayEntry> ();

		public static RelayEntry Get(string name) 
		{
			return RelayEntries [name];
		}
	}
}

