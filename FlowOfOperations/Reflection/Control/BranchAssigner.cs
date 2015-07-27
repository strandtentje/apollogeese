using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class BranchAssigner : ServiceMutator
	{
		public override string Description {
			get {
				return "Assign branches";
			}
		}

		private string sourceServiceIdKey, sourceBranchNameKey,	targetServiceIdKey;

		protected override void Initialize (Settings modSettings)
		{
			sourceServiceIdKey = modSettings.GetString ("sourceserviceidkey", "sourceid");
			sourceBranchNameKey = modSettings.GetString ("sourcebranchnamekey", "sourcebranch");
			targetServiceIdKey = modSettings.GetString ("targetserviceidkey", "targetid");
		}

		protected override bool Process (IInteraction parameters)
		{
			int sourceServiceID = GetServiceInt (parameters, sourceServiceIdKey);
			int targetServiceID = GetServiceInt (parameters, targetServiceIdKey);
			string branchName;
			bool successful;

			try {				
				if (parameters.TryGetFallbackString (sourceBranchNameKey, out branchName)) {
					Service source = GetServiceById (sourceServiceID);
					Service target = GetServiceById (targetServiceID);

					source.Branches [branchName] = target;

					successful = Successful.TryProcess (new MetaInteraction(
						parameters, source, branchName, target));
				} else {
					throw new ControlException (ControlException.Cause.NoBranchSupplied, sourceBranchNameKey);
				}
			} catch (ControlException ex) {
				successful = Failure.TryProcess (new FailureInteraction (ex));
			}

			return successful;
		}
	}
}
