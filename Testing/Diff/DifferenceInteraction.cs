using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.Diagnostics;
using DProcess = System.Diagnostics.Process;

namespace Testing.Diff
{
	class DifferenceInteraction : QuickInteraction
	{
		public DifferenceInteraction (string diffline, IInteraction parent) : base(parent)
		{
			if (diffline.Length > 0) {
				this ["prefix"] = diffline [0];
				if (diffline.Length > 1) {
					this ["line"] = diffline.Substring (1);
				} else {
					this ["line"] = "";
				}
			} else {
				this ["prefix"] = "?";
			}
		}
	}
}

