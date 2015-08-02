using System;
using System.IO;
using System.Text;
using System.Globalization;

namespace BorrehSoft.Utensils.Parsing
{
	/// <summary>
	/// Parser-prototype. Parser modules working with ParsingSession
	/// should adhere to this.
	/// </summary>
	public abstract class Parser
	{	
		object dummy;

		/// <summary>
		/// Run the Parser using the specified Session.
		/// </summary>
		/// <param name='session'>
		/// Session to use.
		/// </param>
		public int Run(ParsingSession session)
		{
			return Run(session, out dummy);
		}

		/// <summary>
		/// Run the specified session and result.
		/// </summary>
		/// <param name='session'>
		/// Session.
		/// </param>
		/// <param name='result'>
		/// Result.
		/// </param>
		public int Run(ParsingSession session, out object result)
		{	
			result = null;
			// this is never good
			if (session.Offset >= session.Data.Length)
				return -1;

			if (this != session.whitespaceParser)
				session.whitespaceParser.Run (session, out dummy);

			if (session.ProfilingEnabled)
				return ProfiledParseMethod (session, out result);
			else 
				return ParseMethod (session, out result);
		}

		public virtual string GetProfileName()
		{
			return this.GetType ().Name;
		}

		internal int ProfiledParseMethod(ParsingSession session, out object result)
		{
			int resultCode = 0;
			object aresult = null;

			session.ParsingProfiler.CheckIn (GetProfileName(), delegate() {
				resultCode = ParseMethod(session, out aresult);
			});

			result = aresult;

			return resultCode;
		}

		/// <summary>
		/// Method which parses data from session into resulting object
		/// </summary>
		/// <returns>
		/// Success value, greater than -1 when succesful.
		/// </returns>
		/// <param name='session'>
		/// ParsingSession to get data from.
		/// </param>
		/// <param name='result'>
		/// Result of Parse Action, if any.
		/// </param>
		internal abstract int ParseMethod(ParsingSession session, out object result);

		public static object GetBestPossible (string stringValue)
		{
			bool boolValue; int intValue; long longValue; double floatValue; 
			object output;

			if (bool.TryParse(stringValue, out boolValue)) {
				output = boolValue;
			} else if (int.TryParse(stringValue, out intValue)) {
			    output = intValue;
			} else if (long.TryParse(stringValue, out longValue)) {
				output = longValue;
			} else if (double.TryParse(stringValue, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out floatValue)) {
				output = floatValue;
			} else {
				output = stringValue;
			}

			return output;
		}

		internal static bool IsAlpha(char character)
		{
			return 
				((character >= 'a') && (character <= 'z')) ||
					((character >= 'A') && (character <= 'Z'));
		}

		internal static bool IsNumeric(char character)
		{
			return
				(character >= '0') && (character <= '9');
		}

		internal static bool IsSpace (char character)
		{
			return " \t".IndexOf(character) > -1;
		}

		internal static bool IsNewline (char character)
		{
			return "\r\n".IndexOf(character) > -1;
		}

		internal static bool IsAlphaNumeric(char character)
		{
			return IsAlpha(character) || IsNumeric(character);
		}

		internal static bool IsAlphaNumericUsc(char character)
		{
			return IsAlphaNumeric (character) || (character == '_');
		}
	}
}

