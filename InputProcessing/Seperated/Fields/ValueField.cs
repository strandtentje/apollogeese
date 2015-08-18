using System;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace InputProcessing
{
	public class ValueField : Service
	{
		public override string Description {
			get {
				throw new NotImplementedException ();
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			base.LoadDefaultParameters (defaultParameter);
		}

		protected override void Initialize (BorrehSoft.Utensils.Collections.Settings.Settings settings)
		{
			base.Initialize (settings);
		}

		protected override void HandleBranchChanged (object sender, BorrehSoft.Utensils.Collections.Maps.ItemChangedEventArgs<Service> e)
		{
			base.HandleBranchChanged (sender, e);
		}

		protected override bool Process (IInteraction parameters)
		{
			return base.Process (parameters);
		}
	}
}

