using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	public class SwitchTemplate : Service
	{
		public override string Description {
			get {
				return "";
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}

		protected override void Initialize (Settings modSettings)
		{

		}

		protected override bool Process (IInteraction parameters)
		{
			return false;
		}
	}
}

