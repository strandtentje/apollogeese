using System;
using BorrehSoft.Utensils.Collections.Settings;

namespace InputProcessing
{
	public class EmailField : TextField
	{
		protected override void Initialize (Settings settings)
		{
			settings["pattern"] = "^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$";
			base.Initialize (settings);
		}
	}
}

