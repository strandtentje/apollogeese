using System;
using System.IO;
using System.Text;

namespace ModularFunk.Parsing
{
	public abstract class Parser
	{	
		public abstract ParseMethod Run { get; }

		public delegate int ParseMethod(string data, ref int offset, out object result);	

		internal static bool IsAlpha(char character)
		{
			return 
				(character >= 'a') && (character <= 'z') &&
				(character >= 'A') && (character <= 'Z');
		}

		internal static bool IsNumeric(char character)
		{
			return
				(character >= '0') && (character <= '9');
		}

		internal static bool IsSpace (char character)
		{
			return (" \t".Contains(character));
		}

		internal static bool IsNewline (char character)
		{
			return "\r\n".IndexOf(character) != -1;
		}

		internal static bool IsAlphaNumeric(char character)
		{
			return IsAlpha(character) || IsNumeric(character);
		}
	}
}

