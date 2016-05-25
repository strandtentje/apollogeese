using System;
using BorrehSoft.Utensils.Collections.Settings;
using System.Globalization;

namespace InputProcessing
{
	/// <summary>
	/// Decimal field.
	/// </summary>
	public class DecimalField : ValueField<decimal>
	{
		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);

			this.Default = settings.GetDecimal ("default", 0);
			this.Min = settings.GetDecimal ("min", decimal.MinValue);
			this.Max = settings.GetDecimal ("max", decimal.MaxValue);
		}

		public override bool TryParse (object serial, out decimal data)
		{
			// i'm so proud of this hack
			return decimal.TryParse (serial.ToString(), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out data);
		}
	}
}

