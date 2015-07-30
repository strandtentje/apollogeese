using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.ApolloGeese.Loader;
using BorrehSoft.Utensils.Collections;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.Module
{
	/// <summary>
	/// Executes into branch from another file.
	/// </summary>
	public class Module : Service
	{
		public override string Description {
			get {
				if (branchName == null)
					return string.Format("{0}:directed", file);
				else 
					return string.Format("{0}:{1}", file, branchName);
			}
		}

		string file, branchName = null;

		public string BranchName { get { return this.branchName; }}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			string[] pathAndBranch = defaultParameter.Split ('@');

			this.Settings["file"] = pathAndBranch [0];

			if (pathAndBranch.Length > 1) {
				this.Settings["branch"] = pathAndBranch [1]; 
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			file = (string)modSettings ["file"];
			if (modSettings.Has("branch"))
				branchName = (string)modSettings.Get ("branch");
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}

		/// <summary>
		/// Gets the branches within the module file 
		/// </summary>
		/// <value>The module branches.</value>
		public Map<Service> ModuleBranches { 
			get {
				return InstanceLoader.GetInstances (file); 
			}
		}

		public Map<object> ModuleMetadata {
			get {
				return InstanceLoader.GetMetadata (file);
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			Service referredService;
			JumpInteraction jumpInteraction;

			string pickedBranchName;

			if (branchName == null) {
				if (parameters is DirectedInteraction) {
					pickedBranchName = ((DirectedInteraction)parameters).BranchName;
				} else {
					throw new JumpException ("{none supplied in settings}");
				}
			} else {
				pickedBranchName = branchName;
			}

			referredService = ModuleBranches [pickedBranchName];
			jumpInteraction = new JumpInteraction (parameters, Branches, GetSettings ());

			return referredService.TryProcess (jumpInteraction);
		}
	}
}

