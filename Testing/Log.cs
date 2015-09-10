using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Collections.Generic;
using BorrehSoft.Utensils.Log;

namespace Testing
{
	public class Log : Service
	{
		public override string Description {
			get {
				return string.Format (
					"Logger \"{0}\" for variables [\"{1}\"]", 
					this.Label, string.Join ("\", \"", this.WatchVariables));
			}
		}

		Service Continue {
			get;
			set;
		}

		IEnumerable<string> WatchVariables {
			get;
			set;
		}

		string Label {
			get;
			set;
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "continue")
				this.Continue = e.NewValue;
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			Settings ["label"] = defaultParameter;
		}

		protected override void Initialize (Settings modSettings)
		{
			this.Label = modSettings.GetString ("label", "noname log");
			this.WatchVariables = modSettings.GetStringList ("variablenames");
			this.Continue = Stub;
		}

		protected override bool Process (IInteraction parameters)
		{
 			foreach (string varName in this.WatchVariables) {
				object candidate;
				string candidateString = "not present";

				if (parameters.TryGetFallback (varName, out candidate)) {
					candidateString = candidate.ToString();
				} 

				Secretary.Report (5, varName, "=", candidateString);
			}

			return this.Continue.TryProcess (parameters);
		}
	}
}

