using System;
using System.IO;

namespace BorrehSoft.Utensils.Parsing.Parsers
{
	public class FilenameParser : StringParser
	{
		public override string ToString ()
		{
			return string.Format ("Like a StringParser, but with an f-prefix that includes the current working directory.");
		}

		internal override int ParseMethod (ParsingSession session, out object result)
		{
			if (session.Data [session.Offset++] == 'f') {
				object fileNameObj;
				string fileName, fullPath;
				base.ParseMethod (session, out fileNameObj);
				fileName = (string)fileNameObj;
				fullPath = Path.Combine (Directory.GetCurrentDirectory (), fileName);
				result = fullPath;
				return fullPath.Length;
			} else {
				session.Offset--;
				result = "";
				return -1;
			}
		}
	}
}

