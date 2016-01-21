using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

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

			ExceptionHandler handler = delegate(
               Service cause, IInteraction context, Exception problem
           ) {
				ErrorHandledInteraction errorInfo;
				errorInfo = new ErrorHandledInteraction (
					cause, context, problem);
				handled = this.CatchBranch.TryProcess(errorInfo);
			};

			ErrorHandlingInteraction wrapper;
			wrapper = new ErrorHandlingInteraction (
				parameters, handler);

			return this.TryBranch.TryProcess (wrapper) || handled;
		}
	}
}

