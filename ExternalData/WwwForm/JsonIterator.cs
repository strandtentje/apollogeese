using System;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.ApolloGeese.CoreTypes;
using Iteration;
using System.IO;
using BorrehSoft.Utilities.Collections.Settings;
using SimpleJson.Transcoder;
using System.Collections;

namespace ExternalData
{
	public class JsonIterator : Service
	{
		IterationBranches iterationHandlers = new IterationBranches ();
		string ArrayVariable;

		public override string Description {
			get {
				return "JSON Iterator";
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			Settings ["arrayvariable"] = defaultParameter;
		}

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);
			this.ArrayVariable = settings.GetString ("arrayvariable");
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (!iterationHandlers.AddBranch (e.Name, e.NewValue)) {
				base.HandleBranchChanged (sender, e);
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			bool success = true;
			string arrayString;
			if (parameters.TryGetFallbackString (ArrayVariable, out arrayString)) {
				var iteration = new BranchingIteration (iterationHandlers);
				var jsonArray = JsonSerializer.DeserializeString (arrayString) as ArrayList;

				foreach (var item in jsonArray) {					
					success &= iteration.Register (
						new SimpleInteraction (
							parameters, 
							this.ArrayVariable, 
							JsonFormulator.Formulate (item)
						)
					);
				}
				success &= iteration.Finish (parameters);
			}
			return success;
		}
	}
}

