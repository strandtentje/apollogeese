using System;
using BorrehSoft.Utilities.Collections.Settings;

namespace InputProcessing
{
	public class NumberField : ValueField<int>
	{
		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);

			this.Default = settings.GetInt ("default", 0);
			this.Min = settings.GetInt ("min", int.MinValue);
			this.Max = settings.GetInt ("max", int.MaxValue);
		}

		public override bool TryParse (object serial, out int data)
		{
			return int.TryParse (serial.ToString (), out data);
		}
	}
}

