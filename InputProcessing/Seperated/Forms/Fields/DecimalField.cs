using System;
using BorrehSoft.Utensils.Collections.Settings;

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

		public override bool TryParse (string serial, out decimal data)
		{
			return decimal.TryParse (serial, out data);
		}
	}
}

