using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;

namespace BetterData
{
	public class Transaction : Communicator
	{
		public override string Description {
			get {
				return "Transaction";
			}
		}

		Service Begin {
			get;
			set;
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "begin") 
				this.Begin = e.NewValue;
		}

		protected override bool Process (IInteraction parameters)
		{
			BlockingPool<IDbCommand> commandPool

			return this.Begin.TryProcess (new TransactionInteraction (parameters, commandPool));
		}
	}
}

