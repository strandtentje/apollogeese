using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;

namespace Crypto
{
	public class Encrypt : SingleBranchService
	{
		public override string Description {
			get {
				return "Don't use this service. I'm serious.";
			}
		}

		AssymetricTransactionFactory TransactionFactory;

		protected override void Initialize (Settings settings)
		{
			this.TransactionFactory = AssymetricTransactionFactory.FromSettings(settings);
		}

		protected override bool Process (IInteraction parameters)
		{
			bool pipeBack = (WithBranch == null) || (WithBranch is StubService);
			var payload = TransactionFactory.FromInteraction(parameters).Encrypt();

			if (pipeBack) {
				Closest<IOutgoingBodiedInteraction>.From(parameters).OutgoingBody.Write(payload, 0, payload.Length);
				return true;
			} else {
				var base64 = Convert.ToBase64String(payload);
				var payloadInteraction = new SimpleInteraction(parameters, TransactionFactory.MessageVariable, base64);
				return WithBranch.TryProcess(payloadInteraction);
			}
		}
	}
}

