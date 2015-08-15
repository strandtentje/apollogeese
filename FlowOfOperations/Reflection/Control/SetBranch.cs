using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class SetBranch : ServiceMutator
	{
		public override string Description {
			get {
				return "Assign branches";
			}
		}

		[Instruction("Variable name in context where id of source service is stored.", "sourceid")]
		public string SourceServiceIdKey { get; set; }

		[Instruction("Variable name in context where branch name of source service is stored.", "sourcebranch")]
		public string SourceBranchNameKey { get; set; }

		[Instruction("Variable name in context where id of target service is stored.", "targetid")]
		public string TargetServiceIdKey { get; set; }

		protected override void Initialize (Settings modSettings)
		{
			SourceServiceIdKey = modSettings.GetString ("sourceserviceidkey", "sourceid");
			SourceBranchNameKey = modSettings.GetString ("sourcebranchnamekey", "sourcebranch");
			TargetServiceIdKey = modSettings.GetString ("targetserviceidkey", "targetid");
		}

		protected override bool Process (IInteraction parameters)
		{
			int sourceServiceID = GetServiceInt (parameters, SourceServiceIdKey);
			int targetServiceID = GetServiceInt (parameters, TargetServiceIdKey);
			string branchName;
			bool successful;

			try {				
				if (parameters.TryGetFallbackString (SourceBranchNameKey, out branchName)) {
					Service source = GetServiceById (sourceServiceID);
					Service target = GetServiceById (targetServiceID);

					source.Branches [branchName] = target;

					successful = Successful.TryProcess (new MetaInteraction(
						parameters, source, branchName, target));
				} else {
					throw new ControlException (ControlException.Cause.NoBranchSupplied, SourceBranchNameKey);
				}
			} catch (ControlException ex) {
				successful = Failure.TryProcess (new FailureInteraction (parameters, ex));
			}

			return successful;
		}
	}
}
