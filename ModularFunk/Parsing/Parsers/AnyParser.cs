using System;
using System.Data.Linq;

namespace BorrehSoft.Utensils.Parsing.Parsers
{
	public class AnyParser : Parser
	{
		public List<Parser> AcceptedParsers { get; set; }

		public override string ToString ()
		{
			return string.Format (
				"One-a those: {0}", 
				string.Join (" or ", AcceptedParsers.ToStringArray ()));
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

