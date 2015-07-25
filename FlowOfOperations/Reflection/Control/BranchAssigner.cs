using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class BranchAssigner : Service
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

		Service Successful { get; set; }

		Service Failure { get; set; }

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "successful")
				this.Successful = e.NewValue;
			if (e.Name == "failure")
				this.Failure = e.NewValue;
		}

		int GetServiceInt (IInteraction parameters, string key)
		{
			object candidateValue;

			if (parameters.TryGetFallback (key, out candidateValue)) {
				if (candidateValue is int) {
					return (int)candidateValue;
				} else {
					if (candidateValue is string) {
						int candidateInt;
						if (int.TryParse ((string)candidateValue, out candidateInt)) {
							return candidateInt;
						} else {
							throw new AssignException (AssignException.Cause.IntParse, key);
						}
					} else {
						throw new AssignException (AssignException.Cause.TypeMismatch, key);
					}
				}
			} else {
				throw new AssignException (AssignException.Cause.NoCandidate, key);
			}
		}

		Service GetServiceById(int serviceID) {
			if (ModelLookup.ContainsKey (serviceID)) {
				return ModelLookup [serviceID];
			} else {
				throw new AssignException (AssignException.Cause.NoService, serviceID.ToString());
			}
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
					throw new AssignException (AssignException.Cause.NoBranchSupplied, sourceBranchNameKey);
				}
			} catch (AssignException ex) {
				QuickInteraction error = new QuickInteraction ();
				error ["message"] = ex.Message;
				successful = Failure.TryProcess (error);
			}

			return successful;
		}
	}
}
