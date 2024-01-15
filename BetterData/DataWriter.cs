using System;
using System.Data;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections;

namespace BetterData
{
	public class DataWriter : Commander
	{
		public override string Description {
			get {
				return string.Format (
					"Write {1} on {0}", 
					DatasourceName,
					base.Description
				);
			}
		}

		IntMap<Service> changeCountBranches = new IntMap<Service> () { Default = Stub };

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			base.HandleBranchChanged (sender, e);

			changeCountBranches.Set (e.Name, e.NewValue, "changed_");

			if (e.Name == "default" || e.Name == "_with") {
				changeCountBranches.Default = e.NewValue;
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			int affectedRows = 0;

			UseCommand (parameters, delegate(IDbCommand command) {				
				affectedRows = command.ExecuteNonQuery ();
			});

			return changeCountBranches[affectedRows].TryProcess (parameters);
		}
	}
}

