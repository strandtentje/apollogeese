using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using System.Data;
using BorrehSoft.Utilities.Collections.Settings;

namespace BetterData
{
	public class Transaction : Commander
	{
		public override string Description {
			get {
				return "Transaction";
			}
		}

		bool Rollback { get; set; }

        protected override void Initialize(Settings settings)
        {
            this.DatasourceName = settings.GetString("connection", "default");
			this.Rollback = settings.GetBool ("rollback", false);
        }

		protected override bool Process (IInteraction parameters)
		{
            bool success = true;

			IDbTransaction actualTransaction = this.ProduceConnection().BeginTransaction();   

			success &= WithBranch.TryProcess(new TransactionInteraction(this.ProduceConnection(), this.DatasourceName, parameters));

			if (this.Rollback) {
				actualTransaction.Rollback ();
			} else {
				actualTransaction.Commit();
			}

            return success;
		}
	}
}

