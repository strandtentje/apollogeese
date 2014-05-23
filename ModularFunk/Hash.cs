using System;

namespace BorrehSoft.Utensils
{
	public class Hash
	{
		public static UInt64 Knuth(byte[] data)
		{
		    UInt64 hashedValue = 3074457345618258791ul;
		    for(int i=0; i < data.Length; i++)
		    {
		        hashedValue += data[i];
		        hashedValue *= 3074457345618258799ul;
		    }

		    return hashedValue;
		}

		public static UInt64 Knuth(string data)
		{
		    UInt64 hashedValue = 3074457345618258791ul;
		    for(int i=0; i < data.Length; i++)
		    {
		        hashedValue += data[i];
		        hashedValue *= 3074457345618258799ul;
		    }

		    return hashedValue;
		}
	}
}

