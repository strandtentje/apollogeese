using System;
using BorrehSoft.Utensils.Collections.Settings;

namespace InputProcessing
{
	public class EmailField : TextField
	{
		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);
			this.Pattern = "^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$";
		}
	}
}

