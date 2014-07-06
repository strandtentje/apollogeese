using System;
using BorrehSoft.Utensils.Parsing;
using BorrehSoft.Utensils.Parsing.Parsers;
using System.Collections.Generic;

namespace BorrehSoft.Utensils.Collections.Settings
{
	/// <summary>
	/// Settings parser.
	/// </summary>
	class StatementParser : ConcatenationParser
	{
		public StatementParser() : base('(', ')', ',')
		{

		}

		public IdentifierParser identifierParser = new IdentifierParser();

		internal override int ParseMethod (ParsingSession session, out object result)
		{
			throw new NotImplementedException();

		}
	}
}