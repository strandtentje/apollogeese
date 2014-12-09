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

		AnyParser ValueParser;
		AssignmentParser AssignmentParser;
		IdentifierParser TypeIDParser;
		ConcatenationParser ModconfParser;

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Utensils.Settings.SettingsParser"/> class.
		/// </summary>
		public SettingsParser(
			char startBlock = '{', char endBlock = '}', char entitySe = ';', 
			char startArr = '[', char endArr = ']', char arrSe = ',',
			char couplerChar = '=') : base(startBlock, endBlock, entitySe)
		{
			ConcatenationParser listParser = new ConcatenationParser (startArr, endArr, arrSe);

			AssignmentParser = new AssignmentParser (couplerChar);

			ValueParser = new AnyParser (
				new ValueParser<int> (int.TryParse), 
				new ValueParser<long> (long.TryParse),
				new ValueParser<float> (float.TryParse),
				new ValueParser<bool> (bool.TryParse, "(True|False|true|false)"), 
				new FilenameParser (),
				new ReferenceParser (),
				new StringParser (), 
				listParser, 
				this
			);			

			TypeIDParser = new IdentifierParser ();
			ModconfParser = new ConcatenationParser ('(', ')', ',');

			listParser.InnerParser = ValueParser;
			AssignmentParser.InnerParser = ValueParser;
			ModconfParser.InnerParser = AssignmentParser;
			this.InnerParser = AssignmentParser;
		}

		private void AssignmentsToSettings(object assignments, Settings target)
		{			
			foreach (object assignment in (assignments as IEnumerable<object>)) {
				Tuple<string, object> t = assignment as Tuple<string, object>;
				target [t.Item1] = t.Item2;
			}
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
			object assignments, uncastTypeid, uncastModconf;
			Settings rootconf = new Settings(), modconf = new Settings ();

			int successCode = -1;
			bool Identifier = TypeIDParser.ParseMethod (session, out uncastTypeid) > 0;

			if (Identifier && (ModconfParser.ParseMethod (session, out uncastModconf) > 0)) {
				string typeid = uncastTypeid as string;
				AssignmentsToSettings (uncastModconf, modconf);

				rootconf ["type"] = typeid;
				rootconf ["modconf"] = modconf;

				successCode = 1;
			}

			if (base.ParseMethod (session, out assignments) > 0) {
				AssignmentsToSettings(assignments, rootconf);

				successCode = 2;
			}

			result = rootconf;

			return successCode;
		}	
	}
}