using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Collections.Generic;
using BorrehSoft.Utensils.Log;

namespace Testing
{
	public class Log : SingleBranchService
	{
		public override string Description {
			get {
				return string.Format (
					"Logger \"{0}\" for variables [\"{1}\"]", 
					this.Label, string.Join ("\", \"", this.WatchVariables));
			}
		}

		IEnumerable<string> WatchVariables {
			get;
			set;
		}

		string Label {
			get;
			set;
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			Settings ["label"] = defaultParameter;
		}

		protected override void Initialize (Settings modSettings)
		{
			this.Label = modSettings.GetString ("label", "noname log");
			this.WatchVariables = modSettings.GetStringList ("variablenames");
		}

		protected override bool Process (IInteraction parameters)
		{
			Secretary.Report (5, this.Label);

 			foreach (string varName in this.WatchVariables) {
				object candidate;
				string candidateString = "not present";

				if (parameters.TryGetFallback (varName, out candidate)) {
					candidateString = candidate.ToString();
				} 

				Secretary.Report (5, varName, "=", candidateString);
			}

			return this.WithBranch.TryProcess (parameters);
		}
	}
}

