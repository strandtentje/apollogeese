using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.Module
{
	/// <summary>
	/// Escapes from the current module, back into one of the child branches
	/// of the calling Module.
	/// </summary>
	public class Return : Service
	{
		private Service defaultBranch = Stub;

		[Instruction("Name of branch in calling module to return control to.")]
		public string BranchName { get; set; }

		public override string Description {
			get {
				return string.Format("Returns to calling module branch {0}", BranchName);
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "default") defaultBranch = e.NewValue ?? Stub;
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			this.Settings ["branchname"] = defaultParameter;
		}

		protected override void Initialize (Settings modSettings)
		{
			BranchName = modSettings.GetString ("branchname");
		}

		public Service GetReturnService(IInteraction parameters, Service defaultService) {
			Service returnService;

			if (TryGetReturnService (parameters, out returnService))
				return returnService;
			else 
				return defaultService;
		}

		protected bool TryGetReturnService(IInteraction parameters, out Service returnService) {
			JumpInteraction jumpParameters = (JumpInteraction)parameters.GetClosest (typeof(JumpInteraction));

			return jumpParameters.TryGetDeepBranch (BranchName, out returnService);
		}

		protected override bool Process (IInteraction parameters)
		{
			Service returnService = GetReturnService (parameters, defaultBranch);

			return returnService.TryProcess (parameters);
		}
	}
}

