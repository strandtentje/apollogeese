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

		public override string GetProfileName ()
		{
			return string.Format ("<{0}>Parser", typeof(T).Name);
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
			// if you're going to put any value longer than 32 characters in the config file,
			// you're doing something wrong.
			int length = Math.Min (32, session.Data.Length - session.Offset);
			Match match = valuePattern.Match(session.Data.Substring(session.Offset, length));

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

