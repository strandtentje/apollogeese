using System;
using BorrehSoft.Utilities.Collections.Settings;

namespace InputProcessing
{
	public class NameField : TextField
	{
		protected override void Initialize (Settings settings)
		{
			settings["pattern"] = "^\\w+([\\s-+.']\\w+)*";
			base.Initialize (settings);
		}
	}
}

