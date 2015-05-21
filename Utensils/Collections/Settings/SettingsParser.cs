using System;
using BorrehSoft.Utensils.Parsing;
using BorrehSoft.Utensils.Parsing.Parsers;
using System.Collections.Generic;
using System.Globalization;

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
		/// Nulls the parser. (Monodevelop generated this documentation and
		/// I can't stop laughing so I'm going to leave this here for now)
		/// </summary>
		/// <returns><c>true</c>, if parser was nulled, <c>false</c> otherwise.</returns>
		/// <param name="data">Data.</param>
		/// <param name="value">Value.</param>
		private bool nullParser(string data, out object value) 
		{
			value = null;
			return data == "null";
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

			AssignmentParser = new AssignmentParser (couplerChar);

			ValueParser = new AnyParser (
				new ValueParser<int> (int.TryParse), 
				new ValueParser<long> (long.TryParse),
				new ValueParser<float> (floatParse),
				new ValueParser<bool> (bool.TryParse, "(True|False|true|false)"), 
				/* new ValueParser<object> (nullParser, "(Null|null|NULL)"),
				 * I choose not to entirely remove this line because I believe it 
				 * is a nice monument to remind us all of the disaster that never 
				 * was. */
				new FilenameParser (),
				new ReferenceParser (),
				new StringParser (), 
				listParser, 
				this
			);			

			TypeIDParser = new IdentifierParser ();
			ModconfParser = new ConstructorParser ();

			listParser.InnerParser = ValueParser;
			AssignmentParser.InnerParser = ValueParser;
			ModconfParser.InnerParser = AssignmentParser;
			this.InnerParser = AssignmentParser;
		}

        private bool floatParse(string data, out float value)
        {
            return float.TryParse(data, NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out value);
        }

		/// <summary>
		/// Bah, wat vies.
		/// </summary>
		/// <param name="assignments">Assignments.</param>
		/// <param name="target">Target.</param>
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
			Settings rootconf, modconf = new Settings ();

			rootconf = session.References [session.ContextName] as Settings;
			if (rootconf == null)
				rootconf = new Settings ();

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