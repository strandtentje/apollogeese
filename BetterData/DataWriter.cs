using System;
using System.Data;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;

namespace BetterData
{
	public class DataWriter : Commander
	{
		public override string Description {
			get {
				return string.Format (
					"Write with {0} on {1}", 
					QueryName, DatasourceName);
			}
		}

		BranchesByNumber changeCountBranches = new BranchesByNumber ("changed");

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			base.HandleBranchChanged (sender, e);

			changeCountBranches.SetBranch (e.Name, e.NewValue);

			if (e.Name == "default") {
				changeCountBranches.DefaultBranch = e.NewValue;
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			int affectedRows;

			RunCommand (parameters, delegate(IDbCommand command) {				
				affectedRows = command.ExecuteNonQuery ();
			});

			return changeCountBranches.Find(affectedRows).TryProcess (parameters);
		}
	}
}

