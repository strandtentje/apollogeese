using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace Validating
{
	public class Check : TwoBranchedService
	{
		public override string Description {
			get {
				return "Branch either successful or failure depending on whether or" +
				"not failures were reported downstream on 'subject'";
			}
		}

		protected override void Initialize (Settings settings)
		{
			
		}

		public Service Subject { get; set; }

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			base.HandleBranchChanged (sender, e);

			if (e.Name == "subject") {
				this.Subject = e.NewValue;
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			CheckInteraction parameters = new CheckInteraction(parameters);

			bool success;

			if (this.Subject.TryProcess(parameters) && parameters.Successful)  {
				success &= this.Successful.TryProcess(parameters);
			} else {
				success &= this.Failure.TryProcess(parameters);
			}

			return success;
		}
	}
}

