using System;
using InputProcessing;
using BorrehSoft.Utilities.Collections.Settings;

namespace Static
{
	public class HexField : TextField
	{
		protected override void Initialize (Settings settings)
		{
			settings["pattern"] = "^([a-f|0-9][a-f|0-9])+$";
			base.Initialize (settings);
		}
	}
}

