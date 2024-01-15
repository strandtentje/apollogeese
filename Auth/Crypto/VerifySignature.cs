using System;
using System.Text;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using ExternalData;

namespace Crypto
{
	public class VerifySignature : TwoBranchedService
	{
		public override string Description {
			get {
				return "verify message using public key and signature";
			}
		}

		AssymetricTransactionFactory TransactionFactory;

		string SignatureVariable;

		protected override void Initialize (Settings settings)
		{
			this.TransactionFactory = AssymetricTransactionFactory.FromSettings(settings);
			this.SignatureVariable = settings.GetString("signature_override", "signature");
		}

		protected override bool Process (IInteraction parameters)
		{	
			byte[] signature = Fallback.FromBase64(parameters, SignatureVariable);

			bool isMessageValid = TransactionFactory.FromInteraction(parameters).VerifySha256Signature(signature);

			if (isMessageValid) {
				return Successful.TryProcess(parameters);
			} else {
				return Failure.TryProcess(parameters);
			}
		}
	}
}

