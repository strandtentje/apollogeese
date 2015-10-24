using System;
using System.Data;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections;

namespace BetterData
{
	public class DataWriter : Commander
	{
		public override string Description {
			get {
				return string.Format (
					"Write on {0}", DatasourceName);


			}
		}

		IntMap<Service> changeCountBranches = new IntMap<Service> () { Default = Stub };

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			base.HandleBranchChanged (sender, e);

			changeCountBranches.Set (e.Name, e.NewValue, "changed_");

			if (e.Name == "default") {
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

