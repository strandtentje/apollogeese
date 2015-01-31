using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.ApolloGeese.Loader;

namespace BorrehSoft.Extensions.FlowOfOperations.Module
{
	/// <summary>
	/// Executes into branch from another file.
	/// </summary>
	public class Module : Service
	{
		public override string Description {
			get {
				if (branchName == null)
					return string.Format("module:{0}:directed", file);
				else 
					return string.Format("module:{0}:{1}", file, branchName);
			}
		}

		string file, branchName = null;

		protected override void Initialize (Settings modSettings)
		{
			file = (string)modSettings ["file"];
			if (modSettings.Has("branch"))
				branchName = (string)modSettings.Get ("branch");
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}

		protected override bool Process (IInteraction parameters)
		{
			Service referredService;
			JumpInteraction jumpInteraction;

			string pickedBranchName;

			if (branchName == null)
				pickedBranchName = ((DirectedInteraction)parameters).BranchName;
			else
				pickedBranchName = branchName;

			referredService = InstanceLoader.GetInstances (file) [pickedBranchName];
			jumpInteraction = new JumpInteraction (parameters, Branches, GetSettings ());

			return referredService.TryProcess (jumpInteraction);
		}
	}
}

