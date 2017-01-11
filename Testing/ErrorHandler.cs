using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Log;

namespace Testing
{
	public class ErrorHandler : Service
	{
		public override string Description {
			get {
				return "Error Handler";
			}
		}

		protected override void Initialize (Settings settings)
		{
			
		}

		Service TryBranch, CatchBranch;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "try") {
				this.TryBranch = e.NewValue;
			} else if (e.Name == "catch") {
				this.CatchBranch = e.NewValue;
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			bool handled = false;
			bool handling = false;

			ExceptionHandler handler = delegate(
               Service cause, IInteraction context, Exception problem
           ) {
				if (handling) {
					Secretary.Report(
						5, 
						"Another error occurred",
						"in the process of handling",
						"an error:", problem.Message);
				} else {						
					handling = true;
					ErrorHandledInteraction errorInfo;
					errorInfo = new ErrorHandledInteraction (
						cause, context, problem);				

					handled = this.CatchBranch.TryProcess(errorInfo);	
				}
			};

			ErrorHandlingInteraction wrapper;
			wrapper = new ErrorHandlingInteraction (
				parameters, handler);

			return this.TryBranch.TryProcess (wrapper) || handled;
		}
	}
}

