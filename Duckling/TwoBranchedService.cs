using System;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Duckling
{
	public abstract class TwoBranchedService : Service
	{
		protected Service Successful { get; private set; }

		protected Service Failure { get; private set; }

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "successful")
				this.Successful = e.NewValue;
			if (e.Name == "failure") 
				this.Failure = e.NewValue;
		}
	}
}

