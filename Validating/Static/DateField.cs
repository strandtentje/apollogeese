using System;
using System.Text;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Text.RegularExpressions;

namespace InputProcessing
{
	public class DateField : Field<string>
	{
		protected override bool CheckValid (object rawInput)
		{
			DateTime dummy;
			return DateTime.TryParse (rawInput.ToString (), out dummy);
		}
	}
}

