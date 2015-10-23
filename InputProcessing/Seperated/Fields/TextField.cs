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

		public override void LoadDefaultParameters (string defaultParameter)
		{
			this.Settings ["pattern"] = defaultParameter;
		}

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);
			this.Pattern = settings.GetString ("pattern");
		}

		protected override Service GetFeedbackForInput (object rawInput, out string value)
		{
			Service feedback = this.Failure;
			string stringInput = rawInput.ToString ();
			
 			value = "";

			if (this.IsRequired && (stringInput.Length == 0)) {
				feedback = this.Missing;
			} else {
				if (this.regex.IsMatch (stringInput)) {
					value = stringInput;
					feedback = this.Successful;				
				} 
			}

			return feedback;
		}
	}
}

