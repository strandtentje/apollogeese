using System;
using System.Data.Linq;
using BorrehSoft.Utensils.Collections;
using System.Collections.Generic;
using System.Text;

namespace BorrehSoft.Utensils.Parsing.Parsers
{
	public class AnyParser : Parser
	{
		public List<Parser> AcceptedParsers { get; set; }

		public override string ToString ()
		{
			StringBuilder stringBuilder = new StringBuilder ();

			foreach (Parser parser in AcceptedParsers)
				stringBuilder.Append (parser + ", ");

			return string.Format (
				"One-a those: {0}", stringBuilder.ToString());
		}

		public AnyParser (params Parser[] AcceptedParsers)
		{
			this.AcceptedParsers = new List<Parser> (AcceptedParsers);
		}

		internal override int ParseMethod (ParsingSession session, out object result)
		{
			for (int iParser = 0; iParser < AcceptedParsers.Count; iParser++)
				if (AcceptedParsers [iParser].Run (session, out result) > 0)
					return iParser;

			result = null;
			return -1;
		}
	}
}

