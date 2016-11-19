using System;
using System.Text;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Text.RegularExpressions;

namespace InputProcessing
{
	public class TextField : Field<string>
	{
		Regex regex;

		[Instruction("Length limit", int.MaxValue)]
		public int MaxLength { get; set; }

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
			this.MaxLength = settings.GetInt ("maxlength", int.MaxValue);
		}

		protected override bool CheckValid (object rawInput)
		{
			string stringInput = rawInput.ToString ();

			bool isValid = stringInput.Length <= this.MaxLength;
			isValid &= (stringInput.Length != 0) || !this.IsRequired;
			isValid &= this.regex.IsMatch (stringInput);

			return isValid;
		}
	}
}

