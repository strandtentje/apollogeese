using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.Extensions.FlowOfOperations.Module
{
	/// <summary>
	/// Escapes from the current module, back into one of the child branches
	/// of the calling Module.
	/// </summary>
	public class Return : Service
	{
		private Service defaultBranch = Stub;
		private string branchName;

		public override string Description {
			get {
				return string.Format("Returns to calling module branch {0}", branchName);
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "default") defaultBranch = e.NewValue ?? Stub;
		}

		protected override void Initialize (Settings modSettings)
		{
			branchName = (string)modSettings.Get ("branchname");
		}

		protected override bool Process (IInteraction parameters)
		{
			JumpInteraction jumpParameters = (JumpInteraction)parameters.GetClosest (typeof(JumpInteraction));

			return (jumpParameters.Branches [branchName] ?? defaultBranch).TryProcess (parameters);
		}
	}
}

