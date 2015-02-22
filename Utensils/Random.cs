using System;
using ClrRnd = System.Random;
using System.Security.Cryptography;

namespace BorrehSoft.Utensils
{
	public static class Random
	{
		private static RandomNumberGenerator rng = new RNGCryptoServiceProvider ();

		public static byte[] GetTrue (int i)
		{
			byte[] bytes = new byte[i];
			rng.GetBytes (bytes);
			return bytes;
		}

		static ClrRnd r = new ClrRnd();

		public static float Get (float f, float s)
		{
			return f + (float)r.NextDouble() * (s - f);
		}

		public static int Get(int f, int s)
		{
			return (int)Get((float)f, (float)s);
		}
	}
}
