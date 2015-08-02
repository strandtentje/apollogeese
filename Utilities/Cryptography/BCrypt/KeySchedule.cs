// Copyright (c) 2006 Damien Miller <djm@mindrot.org>
// Copyright (c) 2007 Derek Slager 
// (http://derekslager.com/blog/posts/2007/10/bcrypt-dotnet-strong-password-hashing-for-dotnet-and-mono.ashx)
// Copyright (c) 2015 Rob Tierolff <teh@borreh.nl>
//
// Permission to use, copy, modify, and distribute this software for any
// purpose with or without fee is hereby granted, provided that the above
// copyright notice and this permission notice appear in all copies.
//
// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
// WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
// MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
// ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
// WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
// ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
// OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Auth.BCrypt
{
	/// <summary>BCrypt implements OpenBSD-style Blowfish password hashing
	/// using the scheme described in "A Future-Adaptable Password Scheme"
	/// by Niels Provos and David Mazieres.</summary>
	/// <remarks>
	/// <para>This password hashing system tries to thwart offline
	/// password cracking using a computationally-intensive hashing
	/// algorithm, based on Bruce Schneier's Blowfish cipher. The work
	/// factor of the algorithm is parametized, so it can be increased as
	/// computers get faster.</para>
	/// <para>To hash a password for the first time, call the
	/// <c>HashPassword</c> method with a random salt, like this:</para>
	/// <code>
	/// string hashed = BCrypt.HashPassword(plainPassword, BCrypt.GenerateSalt());
	/// </code>
	/// <para>To check whether a plaintext password matches one that has
	/// been hashed previously, use the <c>CheckPassword</c> method:</para>
	/// <code>
	/// if (BCrypt.CheckPassword(candidatePassword, storedHash)) {
	///     Console.WriteLine("It matches");
	/// } else {
	///     Console.WriteLine("It does not match");
	/// }
	/// </code>
	/// <para>The <c>GenerateSalt</c> method takes an optional parameter
	/// (logRounds) that determines the computational complexity of the
	/// hashing:</para>
	/// <code>
	/// string strongSalt = BCrypt.GenerateSalt(10);
	/// string strongerSalt = BCrypt.GenerateSalt(12);
	/// </code>
	/// <para>
	/// The amount of work increases exponentially (2**log_rounds), so
	/// each increment is twice as much work. The default log_rounds is
	/// 10, and the valid range is 4 to 31.
	/// </para>
	/// </remarks>
	public class KeySchedule {

	    // Blowfish parameters.
	    private const int BLOWFISH_NUM_ROUNDS = 16;

	    // Expanded Blowfish key.
		private uint[] Cipher;
		private uint[] SubstitutionBox;

	    /// <summary>
	    /// Blowfish encipher a single 64-bit block encoded as two 32-bit
	    /// halves.
	    /// </summary>
	    /// <param name="block">An array containing the two 32-bit half
	    /// blocks.</param>
	    /// <param name="offset">The position in the array of the
	    /// blocks.</param>
	    private void Encipher(uint[] block, int offset) {
			uint roundNumber, newValue, leftWord = block[offset], rightWord = block[offset + 1];


	        leftWord ^= this.Cipher[0];
	        for (roundNumber = 0; roundNumber <= BLOWFISH_NUM_ROUNDS - 2;) {
	            // Feistel substitution on left word
	            newValue = this.SubstitutionBox[(leftWord >> 24) & 0xff];
	            newValue += this.SubstitutionBox[0x100 | ((leftWord >> 16) & 0xff)];
	            newValue ^= this.SubstitutionBox[0x200 | ((leftWord >> 8) & 0xff)];
	            newValue += this.SubstitutionBox[0x300 | (leftWord & 0xff)];
				rightWord ^= newValue ^ this.Cipher[++roundNumber];

	            // Feistel substitution on right word
	            newValue = this.SubstitutionBox[(rightWord >> 24) & 0xff];
	            newValue += this.SubstitutionBox[0x100 | ((rightWord >> 16) & 0xff)];
	            newValue ^= this.SubstitutionBox[0x200 | ((rightWord >> 8) & 0xff)];
	            newValue += this.SubstitutionBox[0x300 | (rightWord & 0xff)];
	            leftWord ^= newValue ^ this.Cipher[++roundNumber];
	        }
	        block[offset] = rightWord ^ this.Cipher[BLOWFISH_NUM_ROUNDS + 1];
	        block[offset + 1] = leftWord;
	    }

	    /// <summary>
	    /// Cycically extract a word of key material.
	    /// </summary>
	    /// <param name="data">The string to extract the data
	    /// from.</param>
	    /// <param name="offset">The current offset into data.</param>
	    /// <returns>The next work of material from data.</returns>
	    private static uint StreamToWord(byte[] data, ref int offset) {

	        uint word = 0;

	        for (int i = 0; i < 4; i++) {
	            word = (word << 8) | data[offset];
	            offset = (offset + 1) % data.Length;
	        }

	        return word;
	    }

	    /// <summary>
	    /// Initialize the Blowfish key schedule.
	    /// </summary>
	    private void Reset() {
			this.Cipher = new uint[BeginData.Cipher.Length];
			BeginData.Cipher.CopyTo(this.Cipher, 0);
			this.SubstitutionBox = new uint[BeginData.SubstitutionBox.Length];
			BeginData.SubstitutionBox.CopyTo(this.SubstitutionBox, 0);
	    }

	    /// <summary>
	    /// Key the Blowfish cipher.
	    /// </summary>
	    /// <param name="key">An array containing the key.</param>
	    private void Mutate(byte[] key) {
			uint[] wordPair = { 0, 0 };
			int cipherLength = this.Cipher.Length, substitutionBoxLength = this.SubstitutionBox.Length;

	        int offset = 0;
	        for (int i = 0; i < cipherLength; i++) {
	            this.Cipher[i] = this.Cipher[i] ^ StreamToWord(key, ref offset);
	        }

	        for (int i = 0; i < cipherLength; i += 2) {
	            Encipher(wordPair, 0);
	            this.Cipher[i] = wordPair[0];
	            this.Cipher[i + 1] = wordPair[1];
	        }

	        for (int i = 0; i < substitutionBoxLength; i += 2) {
	            Encipher(wordPair, 0);
	            this.SubstitutionBox[i] = wordPair[0];
	            this.SubstitutionBox[i + 1] = wordPair[1];
	        }
	    }

	    /// <summary>
	    /// Perform the "enhanced key schedule" step described by Provos
	    /// and Mazieres in "A Future-Adaptable Password Scheme"
	    /// (http://www.openbsd.org/papers/bcrypt-paper.ps).
	    /// </summary>
	    /// <param name="data">Salt information.</param>
	    /// <param name="key">Password information.</param>
	    private void EnhancedMutate(byte[] data, byte[] key) {
			uint[] wordPair = { 0, 0 };
			int cipherLength = this.Cipher.Length, substitutionBoxLength = this.SubstitutionBox.Length;

	        int keyOffset = 0;
	        for (int i = 0; i < cipherLength; i++) {
	            this.Cipher[i] = this.Cipher[i] ^ StreamToWord(key, ref keyOffset);
	        }

	        int dataOffset = 0;
	        for (int i = 0; i < cipherLength; i += 2) {
	            wordPair[0] ^= StreamToWord(data, ref dataOffset);
	            wordPair[1] ^= StreamToWord(data, ref dataOffset);
	            Encipher(wordPair, 0);
	            this.Cipher[i] = wordPair[0];
	            this.Cipher[i + 1] = wordPair[1];
	        }

	        for (int i = 0; i < substitutionBoxLength; i += 2) {
	            wordPair[0] ^= StreamToWord(data, ref dataOffset);
	            wordPair[1] ^= StreamToWord(data, ref dataOffset);
	            Encipher(wordPair, 0);
	            this.SubstitutionBox[i] = wordPair[0];
	            this.SubstitutionBox[i + 1] = wordPair[1];
	        }
	    }

	    /// <summary>
	    /// Perform Bcrypt password hashing scheme.
	    /// </summary>
	    /// <param name="password">The password to hash.</param>
	    /// <param name="salt">The binary salt to hash with the
	    /// password.</param>
	    /// <param name="logRounds">The binary logarithm of the number of
	    /// rounds of hashing to apply.</param>
	    /// <returns>An array containing the binary hashed password.</returns>
	    public byte[] GetHashedBytes(byte[] password, byte[] salt, int logRounds) {

			uint[] CipheredData = new uint[BeginData.CipheredData.Length];
			BeginData.CipheredData.CopyTo(CipheredData, 0);

			int CipheredLength = CipheredData.Length;
			byte[] ResultData;

	        if (logRounds < 4 || logRounds > 31) {
	            throw new ArgumentOutOfRangeException("logRounds", logRounds, null);
	        }

	        int rounds = 1 << logRounds;
			if (salt.Length != Salt.PreconfiguredLength) {
	            throw new ArgumentException("Invalid salt length.", "salt");
	        }

	        Reset();
	        EnhancedMutate(salt, password);

	        for (int i = 0; i < rounds; i++) {
	            Mutate(password);
	            Mutate(salt);
	        }

	        for (int i = 0; i < 64; i++) {
	            for (int j = 0; j < (CipheredLength >> 1); j++) {
	                Encipher(CipheredData, j << 1);
	            }
	        }

	        ResultData = new byte[CipheredLength * 4];
	        for (int i = 0, j = 0; i < CipheredLength; i++) {
	            ResultData[j++] = (byte)((CipheredData[i] >> 24) & 0xff);
	            ResultData[j++] = (byte)((CipheredData[i] >> 16) & 0xff);
	            ResultData[j++] = (byte)((CipheredData[i] >> 8) & 0xff);
	            ResultData[j++] = (byte)(CipheredData[i] & 0xff);
	        }

	        return ResultData;
	    }
	}
}