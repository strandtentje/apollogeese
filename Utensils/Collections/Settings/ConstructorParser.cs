using System;
using BorrehSoft.Utensils.Parsing;
using BorrehSoft.Utensils.Parsing.Parsers;
using System.Collections.Generic;

namespace BorrehSoft.Utensils
{
	public class ConstructorParser : ConcatenationParser
	{
		private Parser valueParser;

		public ConstructorParser (Parser valueParser) : base('(', ')', ',')
		{
			this.valueParser = valueParser;
		}		

		protected override int ParseListBody (ParsingSession session, ref List<object> target)
		{			
			object defaultValue;
			bool thereIsMore = true;

			if (valueParser.ParseMethod (session, out defaultValue) > -1) {
				target.Add (new Tuple<string, object> ("default", (string)defaultValue));
				thereIsMore = (coupler.Run(session) > 0);
			}

			if (thereIsMore) {
				return base.ParseListBody (session, ref target);
			} else {
				return closer.Run(session);
			}
		}
	}
}

