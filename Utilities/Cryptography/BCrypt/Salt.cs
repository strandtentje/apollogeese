using System;
using System.Security.Cryptography;

namespace Auth.BCrypt
{
	/// <summary>
	/// Salt.
	/// </summary>
	public class Salt
	{
		private const int DefaultRoundCount = 10;
		public const int PreconfiguredLength = 16;

		public int Major {
			get;
			private set;
		}

		public string Minor {
			get;
			private set;
		}

		public int RoundCount {
			get;
			private set;
		}

		public byte[] Data {
			get;
			private set;
		}

		/// <summary>
		/// Gets the string terminator to use when terminating the string that is to be hashed
		/// </summary>
		/// <value>The string terminator.</value>
		public string StringTerminator {
			get {
				if ((Minor.Length > 0) && (Minor[1] >= 'a')) {
					return "\0";
				} else {
					return string.Empty;
				}
			}
		}

		public Salt (int major, string minor, int roundCount, byte[] saltData)
		{
			this.Major = major;
			this.Minor = minor;
			this.RoundCount = roundCount;
			this.Data = saltData;
		}

		public string GetBase64Data() {
			return Base64.Encode (this.Data, PreconfiguredLength);
		}

		public override string ToString ()
		{
			return string.Format ("${0}{1}${2}${3}", Major.ToString (), Minor, RoundCount, GetBase64Data ());
		}

		public string ToString(string hashString) {
			return string.Format ("{0}{1}", this.ToString (), hashString);
		}

		/// <summary>
		/// Get a Salt from a salt string
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="saltString">Salt string.</param>
		public static Salt FromString(string saltString) {
			if (saltString == null) throw new ArgumentNullException("saltString");

			string[] saltSections = saltString.Split ('$');
			string saltVersion = saltSections [0];
					string saltVersionMinor = string.Empty;

			if (saltVersion.StartsWith ("2")) {
				if ((saltVersion.Length > 1) && (saltVersion [1] >= 'a')) {
					saltVersionMinor = saltVersion.Substring(1);
				}
			} else {
				throw new ArgumentException ("Non-matching salt version");
			}


			string saltRoundsString = saltSections [1];
			int saltRounds = DefaultRoundCount;

			if ((saltRoundsString.Length > 0) && (saltRoundsString.Length < 3)) {
				try {
					saltRounds = int.Parse (saltRoundsString);
				} catch(Exception ex) {
					throw new ArgumentException ("Invalid salt length", ex);
				}
			}

			string actualSaltString = saltSections [2];
			byte[] saltData = Base64.Decode (actualSaltString, PreconfiguredLength);

			return new Salt (2, saltVersionMinor, saltRounds, saltData);
		}

		public static Salt FromRandomData(int logRounds = DefaultRoundCount) {
			byte[] randomBytes = new byte[PreconfiguredLength];
			RandomNumberGenerator.Create().GetBytes(randomBytes);

			return new Salt (2, "a", logRounds, randomBytes);
		}
	}
}

