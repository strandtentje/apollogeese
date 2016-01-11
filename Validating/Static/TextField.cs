using System;
using System.Text;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Text.RegularExpressions;

namespace InputProcessing
{
	public class TextField : Field<string>
	{
		Regex regex;

		[Instruction("String matching pattern")]
		public string Pattern { 
			get {
				return this.regex.ToString ();
			} 
			set {
				this.regex = new Regex (value);
			}
		}

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);
			this.Pattern = settings.GetString ("pattern");
		}

		protected override bool CheckValid (object rawInput)
		{
			string stringInput = rawInput.ToString ();

			bool isValid = (stringInput.Length != 0) || !this.IsRequired;
			isValid &= this.regex.IsMatch (stringInput);

			return isValid;
		}
	}
}

