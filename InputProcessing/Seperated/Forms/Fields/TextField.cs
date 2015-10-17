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
			Settings ["pattern"] = defaultParameter;
		}

		protected override void Initialize (Settings settings)
		{
			this.Pattern = settings.GetString ("pattern");
		}

		override Service FindActionForValue (object valueCandidate, out string value)
		{

		}
	}
}

