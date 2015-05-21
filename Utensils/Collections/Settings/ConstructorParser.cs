using System;
using BorrehSoft.Utensils.Parsing;
using BorrehSoft.Utensils.Parsing.Parsers;
using System.Collections.Generic;

namespace BorrehSoft.Utensils
{
	public class ConstructorParser : ConcatenationParser
	{
		public ConstructorParser () : base('(', ')', ',')
		{

		}		

		protected override int ParseListBody (ParsingSession session, ref List<object> target)
		{			
			StringParser defaultParameterParser = new StringParser ();
			object defaultValue;
			bool thereIsMore = true;

			if (defaultParameterParser.ParseMethod (session, out defaultValue) > -1) {
				target.Add (new Tuple<string, object> ("default", (string)defaultValue));
				thereIsMore = (coupler.Run(session) > 0);
			}

			if (thereIsMore) {
				return base.ParseListBody (session, ref target);
			} else {
				return 1;
			}
		}
	}
}

