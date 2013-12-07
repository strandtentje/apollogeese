using System;
using System.Text;

namespace ModularFunk.Parsing
{
	public class IdentifierParser : Parser
	{
		public override string ToString ()
		{
			return "Identifier";
		}

		public override ParseMethod Run {
			get {
				return ParseIdentifier;
			}
		}
		
		/// <summary>
		/// Tries to parse an Identifier
		/// </summary>
		/// <returns>
		/// The parsed result
		/// </returns>
		public static int ParseIdentifier (string data, ref int offset, out object result)
		{
			int position = offset;

			StringBuilder resultBuilder = new StringBuilder ();

			result = "";

			if (IsAlpha (data [position]))
				resultBuilder.Append (data [position++]);
			else 
				return -1;

			for (offset = position;
				(offset < data.Length) && IsAlphaNumeric(data[position]);
				offset++) {
				resultBuilder.Append (data [position]);
			}

			result = resultBuilder.ToString();
			return resultBuilder.Length;
		}			
	}
}

