using System;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public abstract class SingleBranchService : Service
	{
		private Service withBranch = StubService.Instance;

		protected Service WithBranch { get { return withBranch; } }

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			switch (e.Name) {
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
				Secretary.Report (5, "using deprecated single branch at", ConfigLine);
				this.withBranch = e.NewValue;
				break;
			case SingleBranchNames.With:
				this.withBranch = e.NewValue;
				break;
			default:
				break;
			}
		}
	}
}

