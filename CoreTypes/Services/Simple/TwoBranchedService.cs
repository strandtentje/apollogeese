using System;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public abstract class TwoBranchedService : Service
	{
		protected Service Successful { get; set; }

		protected Service Failure { get; set; }

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "successful")
				this.Successful = e.NewValue;
			if (e.Name == "failure") 
				this.Failure = e.NewValue;
		}
	}
}

