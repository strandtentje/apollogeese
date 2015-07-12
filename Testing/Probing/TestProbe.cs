using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;

namespace Testing
{
	public class TestProbe : Service
	{
		public override string Description {
			get {
				return "Probe for testcase";
			}
		}

		private string ProbeName { get { return this.Settings.GetString ("default", "anonymousprobe"); } }

		protected override bool Process (IInteraction parameters)
		{
			TestContextInteraction testContext = (TestContextInteraction)parameters.GetClosest (typeof(TestContextInteraction));

			Settings probeValues = this.Settings.GetSubsettings (testContext.OriginatingCaseName);

			foreach (string key in probeValues.Dictionary.Keys) {
				object measuredValue = null;
				ProbeResultInteraction probeResult;

				parameters.TryGetFallback (key, out measuredValue);
				probeResult = new ProbeResultInteraction (
					parameters, this.ProbeName, key, 
					probeValues [key], measuredValue);

				testContext.ProbeResults.Add (probeResult);
			}

			return (Branches ["continue"] ?? Stub).TryProcess (parameters);
		}
	}
}

