using System;
using System.Text;

namespace Auth.BCrypt
{
	/// <summary>
	/// Password.
	/// </summary>
	public class Password
	{
		/// <summary>
		/// Gets the password in plaintext.
		/// </summary>
		/// <value>The password in plaintext.</value>
		public string Plaintext { 
			get;
			private set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Auth.BCrypt.Password"/> class.
		/// </summary>
		/// <param name="plaintext">Plaintext password</param>
		public Password (string plaintext)
		{
			if (plaintext == null)
				throw new ArgumentException ("plaintext password is required for construction");

			this.Plaintext = plaintext;
		}

		/// <summary>
		/// Gets a new password hash.
		/// </summary>
		/// <returns>The hash.</returns>
		public string GetHash() {
			return GetHash (Salt.FromRandomData ());
		}

		/// <summary>
		/// Reconstructs a password hash using a known salt
		/// </summary>
		/// <returns>The hash.</returns>
		/// <param name="hashSalt">Hash salt.</param>
		public string GetHash(Salt hashSalt) {
			KeySchedule hasher = new KeySchedule();

			byte[] passwordBytes = Encoding.UTF8.GetBytes(this.Plaintext + hashSalt.StringTerminator);

			byte[] hashedBytes = hasher.GetHashedBytes(passwordBytes, hashSalt.Data, hashSalt.RoundCount);

			string hashString = Base64.Encode (hashedBytes, (BeginData.CipheredData.Length * 4) - 1);

			return hashSalt.ToString (hashString);
		}

		public bool MatchHash(string hash) {
			Salt knownSalt = Salt.FromString (hash);
			return GetHash (knownSalt) == hash;
		}
	}
}

