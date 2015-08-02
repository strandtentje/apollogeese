using System;
using BorrehSoft.Utensils.Parsing.Parsers;
using BorrehSoft.Utensils.Parsing;
using System.Collections.Generic;

namespace BorrehSoft.Utensils.Collections.Settings
{
	public class StructAssignmentParser : AssignmentParser
	{
		private string CouplerSuffix { get; set; }
		List<CharacterParser> couplerParsers = new List<CharacterParser>();

		public StructAssignmentParser(string couplerString = "->", string couplerSuffix = "_branch", char regularCoupler = '=') : base(regularCoupler) {
			foreach (char couplerPart in couplerString) {
				couplerParsers.Add (new CharacterParser (couplerPart));
			}

			this.CouplerSuffix = couplerSuffix;
		}

		protected override bool Couple (ParsingSession session, ref object identifier)
		{
			if (!base.Couple (session, ref identifier)) {
				foreach (CharacterParser parser in couplerParsers) {
					if (parser.Run (session) < 0) {
						return false;
					}
				}

				identifier = string.Concat (identifier, this.CouplerSuffix);
			}

			return true;
		}
	}
}

