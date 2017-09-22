using System;
using BorrehSoft.Utilities.Collections.Settings;
using System.Security.Cryptography;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Text;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;

namespace Crypto
{
	public class AssymetricTransasction
	{
		RSACryptoServiceProvider Provider;
		byte[] Message;

		public AssymetricTransasction (RSACryptoServiceProvider provider, byte[] message)
		{
			this.Provider = provider; 
			this.Message = message;
		}

		public bool VerifySha256Signature(byte[] signature) 
		{
			return Provider.VerifyData(Message, "sha256", signature);
		}

		public byte[] Encrypt ()
		{
			return Provider.Encrypt(this.Message, RSAEncryptionPadding.OaepSHA1);
		}
	}

	public class AssymetricTransactionFactory
	{
		RSACryptoServiceProvider CryptoAlgorithm;
		string PublicKeyVariable;
		public string MessageVariable { get; private set; }
		bool IsRawBase64Message, IsUnicodeTextMessage;

		public AssymetricTransactionFactory (RSACryptoServiceProvider algo, string publicVar, bool isBase64, bool isUnicode, string msgVar)
		{
			this.CryptoAlgorithm = algo;
			this.PublicKeyVariable = publicVar;
			this.MessageVariable = msgVar;
			this.IsRawBase64Message = isBase64;
			this.IsUnicodeTextMessage = isUnicode;
		}

		public AssymetricTransasction FromInteraction(IInteraction parameters) {
			byte[] publicKey = Fallback.FromBase64(parameters, PublicKeyVariable);
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

			return new AssymetricTransasction(provider, message);
		}

		public static AssymetricTransactionFactory FromSettings(Settings settings) 
		{
			var encoding = settings.GetString ("encoding", "base64");
			return new AssymetricTransactionFactory(
				new RSACryptoServiceProvider(settings.GetInt("keysize", 2048)),
				settings.GetString("publickey_override", "publickey"),
				encoding == "base64", encoding == "unicode",
				settings.GetString("message_override", "message")
			);
		}
	}
}

