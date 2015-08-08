using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace Auth
{
	public class MakePassword : Service
	{
		public override string Description {
			get {
				return "Password hasher";
			}
		}

		protected override void Initialize (Settings modSettings)
		{

		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}

		protected override bool Process (IInteraction parameters)
		{
			return false;
		}
	}
}

