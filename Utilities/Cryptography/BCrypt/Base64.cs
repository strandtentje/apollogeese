using System;
using System.Text;
using System.Collections.Generic;

namespace Auth.BCrypt
{
	public static class Base64
	{

		// Table for Base64 encoding.
		private static readonly char[] Base64ValueDigits = {
			'.', '/', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
			'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V',
			'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h',
			'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
			'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5',
			'6', '7', '8', '9'
		};

		// Table for Base64 decoding.
		private static readonly byte[] Base64DigitValues = {
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 0, 1, 54, 55,
			56, 57, 58, 59, 60, 61, 62, 63, 255, 255,
			255, 255, 255, 255, 255, 2, 3, 4, 5, 6,
			7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
			17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27,
			255, 255, 255, 255, 255, 255, 28, 29, 30,
			31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
			41, 42, 43, 44, 45, 46, 47, 48, 49, 50,
			51, 52, 53, 255, 255, 255, 255, 255
		};

		private static char GetBase64ValueCharacter(int value) {
			return Base64ValueDigits [value & 0x3f];
		}

		/// <summary>Encode a byte array using bcrypt's slightly-modified
		/// Base64 encoding scheme. Note that this is _not_ compatible
		/// with the standard MIME-Base64 encoding.</summary>
		/// <param name="d">The byte array to encode</param>
		/// <param name="length">The number of bytes to encode</param>
		/// <returns>A Base64-encoded string</returns>
		public static string Encode(byte[] data, int length) {

			if (length <= 0 || length > data.Length) {
				throw new ArgumentOutOfRangeException("length", length, null);
			}

			StringBuilder result = new StringBuilder(length * 2);

			// bits represented by letters 
			// for clarity
			// 128    1 128    1 128    1
			// ABCDEFGH IJKLMNOP QRSTUVWX

			int 
				firstByte, firstSixBits, nextTwoBits, 
				secondByte, firstHalf, firstSecondOverlap, secondHalf, 
				thirdByte, firstTwoBits, secondThirdOverlap, lastSixBits;

			for (int offset = 0; offset < length; ) {
				firstByte = data[offset++] & 0xff;              // ABCDEFGH
				firstSixBits = (firstByte >> 2) & 0x3f;         // xxABCDEF
				result.Append(GetBase64ValueCharacter(firstSixBits));

				nextTwoBits = (firstByte & 0x03) << 4;          // xxGHxxxx
				if (offset >= length) {
					result.Append(GetBase64ValueCharacter(nextTwoBits));
					break;
				}

				secondByte = data[offset++] & 0xff;             // IJKLMNOP
				firstHalf = (secondByte >> 4) & 0x0f;           // xxxxIJKL
				firstSecondOverlap = nextTwoBits | firstHalf;   // xxGHIJKL
				result.Append(GetBase64ValueCharacter(firstSecondOverlap));

				secondHalf = (secondByte & 0x0f) << 2;          // xxMNOPxx
				if (offset >= length) {
					result.Append(GetBase64ValueCharacter(secondHalf));
					break;
				}

				thirdByte = data[offset++] & 0xff;              // QRSTUVWX
				firstTwoBits = (thirdByte >> 6) & 0x03;         // xxxxxxQR
				secondThirdOverlap = secondHalf | firstTwoBits; // xxMNOPQR
				lastSixBits = thirdByte & 0x3f;                 // xxSTUVWX

				result.Append(GetBase64ValueCharacter(secondThirdOverlap));
				result.Append(GetBase64ValueCharacter(lastSixBits));
			}

			return result.ToString();
		}

		/// <summary>Look up the 3 bits base64-encoded by the specified
		/// character, range-checking against the conversion
		/// table.</summary>
		/// <param name="c">The Base64-encoded value</param>
		/// <returns>The decoded value of <c>x</c></returns>
		private static int GetBase64CharacterValue(char c) {
			int i = (int)c;
			return (i < 0 || i > Base64DigitValues.Length) ? -1 : Base64DigitValues[i];
		}

		/// <summary>Decode a string encoded using BCrypt's Base64 scheme to a
		/// byte array. Note that this is _not_ compatible with the standard
		/// MIME-Base64 encoding.</summary>
		/// <param name="s">The string to decode</param>
		/// <param name="maximumLength">The maximum number of bytes to decode</param>
		/// <returns>An array containing the decoded bytes</returns>
		public static byte[] Decode(string encodedInput, int maximumLength) {

			List<byte> decodedResult = new List<byte>(Math.Min(maximumLength, encodedInput.Length));

			if (maximumLength <= 0) {
				throw new ArgumentOutOfRangeException("maximumLength", maximumLength, null);
			}

			int 
				firstValue, secondValue, thirdValue,
				firstByte, firstSixBits, nextTwoBits, 
				secondByte, firstHalf, firstSecondOverlap, secondHalf, 
				thirdByte, firstTwoBits, secondThirdOverlap, lastSixBits;

			int inputLength = encodedInput.Length;
			int length = 0;

			// bits represented by letters for clarity
			// 64   1 64   1 64   1 64   1
			// ABCDEF GHIJKL MNOPQR STUVWX

			for (int offset = 0; offset < inputLength - 1 && length < maximumLength; ) {
				firstValue = GetBase64CharacterValue(encodedInput[offset++]);    // ABCDEF
				secondValue = GetBase64CharacterValue(encodedInput[offset++]);   // GHIJKL
				if (firstValue == 255 || secondValue == 255) {
					// Input characters were no valid base64 digits
					break;
				}

				firstSixBits = firstValue << 2; // ABCDEFxx
				nextTwoBits = (secondValue & 0x30) >> 4; // xxxxxxGH
				firstByte = firstSixBits | nextTwoBits; // ABCDEFGH

				decodedResult.Add ((byte)(firstByte));

				if (++length >= maximumLength || offset >= encodedInput.Length) {
					break;
				}

				thirdValue = GetBase64CharacterValue(encodedInput[offset++]);   // MNOPQR
				if (thirdValue == 255) {
					break;
				}

				firstHalf = (secondValue & 0x0f) << 4; // IJKLxxxx
				secondHalf = (thirdValue & 0x3c) >> 2; // xxxxMNOP

				decodedResult.Add ((byte)(firstHalf | secondHalf));
				if (++length >= maximumLength || offset >= encodedInput.Length) {
					break;
				}

				lastSixBits = GetBase64CharacterValue(encodedInput[offset++]);  // STUVWX
				firstTwoBits = (thirdValue & 0x03) << 6;
	
				decodedResult.Add ((byte)(firstTwoBits | lastSixBits));

				++length;
			}

			return decodedResult.ToArray();
		}
	}
}

