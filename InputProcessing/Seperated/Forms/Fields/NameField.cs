using System;
using BorrehSoft.Utensils.Collections.Settings;

namespace InputProcessing
{
	public class NameField : TextField
	{
		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);
			this.Pattern = "^\\w+([\\s-+.']\\w+)*";
		}
	}
}

