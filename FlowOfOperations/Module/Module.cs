using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.ApolloGeese.Loader;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Log;

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

		[Instruction("Name of the context variable wherefrom the branch name should be acquired")]
		public string BranchVariable { get; set; }

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

			if (modSettings.Has ("branchvariable")) {
				this.BranchVariable = modSettings.GetString ("branchvariable");
			} else {
				this.BranchVariable = "branchname";
			}

            this.AutoInvoke = modSettings.GetBool("autoinvoke", false);
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}

        public override void OnReady()
        {
            if (this.AutoInvoke)
            {
                if (this.BranchName != null)
                {
                    if (!ModuleBranches[this.BranchName].TryProcess(new JumpInteraction(null, Branches, GetSettings())))
                    {
                        Secretary.Report(5, "Autoinvoke branch", this.BranchName, "failed");
                    }
                }
                else
                {
                    Secretary.Report(5, "Can't autoinvoke", Description, "- branch needs explicit stating.");
                }
            }
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

        public bool AutoInvoke { get; private set; }

        class ModuleBranchException : Exception
		{
			public ModuleBranchException(string branchVariable) : base(
				string.Format("No branch name found in context at {0}", 
					branchVariable)) {

				}
		}

		protected override bool Process (IInteraction parameters)
		{
			Service referredService;
			JumpInteraction jumpInteraction;

			string pickedBranchName;

			if (BranchName == null) {
				if (!parameters.TryGetFallbackString(this.BranchVariable, out pickedBranchName)) {
					throw new ModuleBranchException (this.BranchVariable);
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
	