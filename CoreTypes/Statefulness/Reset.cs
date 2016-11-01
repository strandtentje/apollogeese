using System;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public class Reset : Service
	{
		Service subject;

		public override string Description {
			get {
				if (subject == null) {
					return "Service Resetter";
				} else {
					return string.Format ("Resetter for '{0}'", subject.Description);
				}
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "subject") {
				this.subject = e.NewValue;
			}			
		}

		protected override bool Process (IInteraction parameters)
		{
			return this.subject.TryProcess (new ResetInteraction (parameters));
		}
	}
}

