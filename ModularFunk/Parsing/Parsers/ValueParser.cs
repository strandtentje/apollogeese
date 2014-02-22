using System;
using System.Text.RegularExpressions;

namespace BorrehSoft.Utensils.Parsing.Parsers
{
	public class ValueParser<T> : Parser
	{
		public delegate bool TryParse(string data, out T value);
		private TryParse tryParse;
		private Regex valuePattern;

		public ValueParser (TryParse tryParse, string regexMatch = "[-+]?[0-9]*\\.?[0-9]+")
		{
			this.tryParse = tryParse;
			this.valuePattern = new Regex(regexMatch);
		}

		/// <summary>
		///  Method which parses data from session into resulting value of earlier
	    ///  supplied type.
		/// </summary>
		/// <returns>
		///  Success value, greater than -1 when succesful. 
		/// </returns>
		/// <param name='session'>
		///  ParsingSession to get data from. 
		/// </param>
		/// <param name='result'>
		///  Result of Parse Action, if any. 
		/// </param>
		internal override int ParseMethod (ParsingSession session, out object result)
		{
			Match match = valuePattern.Match(session.Data.Substring(session.Offset));

			T aValue;

			if (match.Success && 					 // We matched a thing!
				(match.Index == 0) && 				 // It was the next thing
				tryParse (match.Value, out aValue)) // The parser also thought fondly of it 
			{ 
				result = aValue;
				session.Offset += match.Length; 	 // The cursor moves on
				return 1; 							 // We report success!
			}

			result = null;
			return -1;
		}
	}
}

