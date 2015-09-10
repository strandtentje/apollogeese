using System;
using BorrehSoft.ApolloGeese.CoreTypes;
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
				if (BranchName == null)
					return string.Format("{0}:directed", File);
				else 
					return string.Format("{0}:{1}", File, BranchName);
			}
		}

		[Instruction("Module file to load.")]
		public string File { get; set; }

		[Instruction("Name of branch in module to fire up.")]
		public string BranchName { get; set; }

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
			this.File = modSettings.GetString ("file");

			if (modSettings.Has ("branch"))
				this.BranchName = modSettings.GetString ("branch");
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
				return InstanceLoader.GetInstances (File); 
			}
		}

		public Map<object> ModuleMetadata {
			get {
				return InstanceLoader.GetMetadata (File);
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			Service referredService;
			JumpInteraction jumpInteraction;

			string pickedBranchName;

			if (BranchName == null) {
				if (parameters is DirectedInteraction) {
					pickedBranchName = ((DirectedInteraction)parameters).BranchName;
				} else {
					throw new JumpException ("{none supplied in settings}");
				}
			} else {
				pickedBranchName = BranchName;
			}

			referredService = ModuleBranches [pickedBranchName];
			jumpInteraction = new JumpInteraction (parameters, Branches, GetSettings ());

			return referredService.TryProcess (jumpInteraction);
		}
	}
}

