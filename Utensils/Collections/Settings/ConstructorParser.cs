using System;
using BorrehSoft.Utensils.Parsing;
using BorrehSoft.Utensils.Parsing.Parsers;

namespace BorrehSoft.Utensils
{
	public class ConstructorParser : ConcatenationParser
	{
		public ConstructorParser () : base('(', ')', ',')
		{

		}
	}
}

