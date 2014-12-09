using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;

namespace Designer
{
	public class Anchor : Service
	{
		string desc;
		Service proceed;

		public override string Description {
			get {
				return desc;
			}
		}


		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "proceed") proceed = e.NewValue;
		}

		protected override void Initialize (Settings modSettings)
		{
			desc = modSettings.GetString("title", "");
		}

		protected override bool Process (IInteraction parameters)
		{
			return proceed.TryProcess(parameters);
		}
	}
}

