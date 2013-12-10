using System;

namespace ModularFunk.Parsing.Parsers
{
	public class ValueParser<T> : Parser
	{
		public delegate bool TryParse(string data, out T value);
		private TryParse tryParse;

		public ValueParser (ParsingSession session, TryParse tryParse) : base(session)
		{
			this.tryParse = tryParse;
		}

		private override int ParseMethod (ParsingSession session, out object result)
		{
			int endPosition = session.Data.IndexOf (";", session.Offset);
			string entireValue = session.Data.Substring (session.Offset, endPosition - session.Offset);

			if (tryParse (entireValue, out result)) {
				session.Offset = endPosition + 1;
				return 1;
			}
			
			result = null;
			return -1;
		}
	}
}

