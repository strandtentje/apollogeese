using System;
using System.Text;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using ExternalData;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Mono.Security;
using Mono.Security.Cryptography;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;

namespace Crypto
{
	public class VerifySignature : TwoBranchedService
	{
		public override string Description {
			get {
				return "verify message using public key and signature";
			}
		}

		RSACryptoServiceProvider CryptoAlgorithm;

		string PublicKeyVariable, SignatureVariable, MessageVariable;

		bool IsRawBase64Message, IsUnicodeTextMessage;

		protected override void Initialize (Settings settings)
		{
			this.CryptoAlgorithm = new RSACryptoServiceProvider(settings.GetInt("keysize", 2048));
			this.PublicKeyVariable = settings.GetString("publickey_override", "publickey");
			this.SignatureVariable = settings.GetString("signature_override", "signature");
			var encoding = settings.GetString ("encoding", "base64");
			this.IsRawBase64Message = encoding == "base64";
			this.IsUnicodeTextMessage = encoding == "unicode";
			this.MessageVariable = settings.GetString("message_override", "message");
		}

		protected override bool Process (IInteraction parameters)
		{			
			byte[] publicKey = Fallback.FromBase64(parameters, PublicKeyVariable);
			byte[] signature = Fallback.FromBase64(parameters, SignatureVariable);
			byte[] message;

			if (IsUnicodeTextMessage) message = Encoding.UTF8.GetBytes(Fallback<string>.From(parameters, MessageVariable));
			else if (IsRawBase64Message) message = Fallback.FromBase64(parameters, MessageVariable);
			else throw new EncoderFallbackException("No proper encoding found");

			var bcKey = PublicKeyFactory.CreateKey(publicKey) as RsaKeyParameters;
			var provider = new RSACryptoServiceProvider();
			provider.ImportParameters(new RSAParameters() {
				Modulus = bcKey.Modulus.ToByteArrayUnsigned(),
				Exponent = bcKey.Exponent.ToByteArrayUnsigned(),
			});

			bool isMessageValid = provider.VerifyData(message, "sha256", signature);

			if (isMessageValid) {
				return Successful.TryProcess(parameters);
			} else {
				return Failure.TryProcess(parameters);
			}
		}
	}
}

