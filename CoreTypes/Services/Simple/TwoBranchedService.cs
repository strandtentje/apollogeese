using System;
using BorrehSoft.Utilities.Collections.Maps;

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

		protected bool FailForException(IInteraction parameters, Exception ex) 
		{
			if (this.Failure == null) {
				throw new Exception ("Exception happened but no failurebranch", ex);
			} else {
				return this.Failure.TryProcess (
					new SimpleInteraction (
						parameters, 
						"exceptionmessage", 
						ex.Message
					)
				);
			}
		}
	}
}

