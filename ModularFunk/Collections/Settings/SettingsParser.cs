using System;
using BorrehSoft.Utensils.Parsing;
using BorrehSoft.Utensils.Parsing.Parsers;
using System.Collections.Generic;

namespace BorrehSoft.Utensils.Collections.Settings
{
	/// <summary>
	/// Settings parser.
	/// </summary>
	public class SettingsParser : ConcatenationParser
	{
		public override string ToString ()
		{
			return "Settings, accolade-enclosed block with zero or more assignments";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Utensils.Settings.SettingsParser"/> class.
		/// </summary>
		public SettingsParser(
			char startBlock = '{', char endBlock = '}', char entitySe = ';', 
			char startArr = '[', char endArr = ']', char arrSe = ',',
			char couplerChar = '=') : base(startBlock, endBlock, entitySe)
		{
			ConcatenationParser listParser = new ConcatenationParser (startArr, endArr, arrSe);

			AssignmentParser assignmentParser = new AssignmentParser (couplerChar);


			AnyParser valueParser = new AnyParser (
				new ValueParser<long> (long.TryParse),
				new ValueParser<int> (int.TryParse), 
				new ValueParser<float> (float.TryParse),
				new ValueParser<bool> (bool.TryParse, "(True|False|true|false)"), 
				new FilenameParser (),
				new ReferenceParser (),
				new StringParser (), 
				listParser, 
				this
			);			

			listParser.InnerParser = valueParser;
			assignmentParser.InnerParser = valueParser;

			this.InnerParser = assignmentParser;
		}

		/// <summary>
		/// Parsing Method for the <see cref="BorrehSoft.Utensils.Settings"/> type.
		/// </summary>
		/// <returns>
		/// Succes value; zero or higher when succesful.
		/// </returns>
		/// <param name='session'>
		/// Session in which this parsing action will be conducted.
		/// </param>
		/// <param name='result'>
		/// Result of this parsing action
		/// </param>
		internal override int ParseMethod (ParsingSession session, out object result)
		{
			object assignments;

			if (base.ParseMethod (session, out assignments) > 0) {

				Settings map = new Settings ();

				foreach (object assignment in (assignments as IEnumerable<object>)) {
					Tuple<string, object> t = assignment as Tuple<string, object>;
					map [t.Item1] = t.Item2;
				}

				result = map;
				return 1;
			}

			result = null;
			return -1;
		}	
	}
}