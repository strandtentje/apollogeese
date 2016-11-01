using System;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public abstract class SingleBranchService : Service
	{
		protected Service WithBranch { get; private set; }

		protected override sealed void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			switch (e.Name) {
			case SingleBranchNames.With:
			case SingleBranchNames.Begin:
			case SingleBranchNames.Content:
			case SingleBranchNames.Continue:
			case SingleBranchNames.Data:
			case SingleBranchNames.Http:
			case SingleBranchNames.Init:
			case SingleBranchNames.Next:
			case SingleBranchNames.Offload:
			case SingleBranchNames.Proceed:
			case SingleBranchNames.Request:
			case SingleBranchNames.Source:
			case SingleBranchNames.Subject:
				this.WithBranch = e.NewValue;
			default:
				break;
			}
		}
	}
}

