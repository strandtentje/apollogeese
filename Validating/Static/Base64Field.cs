using System;
using InputProcessing;
using BorrehSoft.Utilities.Collections.Settings;

namespace Validating
{
	public class Base64Field : TextField
	{
		protected override void Initialize (Settings settings)
		{
			settings["pattern"] = "[^-A-Za-z0-9+/=]|=[^=]|={3,}$";
			base.Initialize (settings);
		}

		protected override bool CheckValid (object rawInput)
		{
			string stringInput = rawInput.ToString ();

			bool isValid = stringInput.Length <= this.MaxLength;
			isValid &= (stringInput.Length != 0) || !this.IsRequired;

			if (isValid) try {
				Convert.FromBase64String(stringInput);
			} catch (Exception ex) {
				isValid = false;
			}

			return isValid;
		}
	}
}

