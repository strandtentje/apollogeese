using System;

namespace BorrehSoft.Utensils
{
	public struct ParsingBookmark
	{
		public ParsingBookmark(int offset, int line, int col)
		{
			this.Offset = offset;
			this.Line = line;
			this.Column = col;
		}

		public int Offset, Line, Column;
	}
}

