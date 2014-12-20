using System;
using ClrRnd = System.Random;

namespace BorrehSoft.Utensils
{
	public static class Random
	{
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
