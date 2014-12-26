using System;
using System.Text;
using BorrehSoft.ApolloGeese.Duckling;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.Duckling.Http;
using System.Web;
using BorrehSoft.Utensils.Collections;

namespace BorrehSoft.Extensions.BasicWeblings
{
	class VerificationInteraction : QuickInteraction
	{
		public Dictionary<string, Regex> FieldExpressions { get; private set; }
		public List<string> FaultyFields { get; private set; }
		public bool HtmlEscape { get; set; }

		public VerificationInteraction (IInteraction parameters, Dictionary<string, Regex> fieldExpressions) : base(parameters)
		{
			this.FieldExpressions = fieldExpressions;
		}

		/// <summary>
		/// Loads the fields.
		/// </summary>
		/// <param name="postedData">Posted data.</param>
		/// <param name="fieldDefaults">Field defaults.</param>
		public void LoadFields(Map<object> postedData, Map<string> fieldDefaults)
		{			
			FaultyFields = new List<string> ();

			foreach (string fieldName in FieldExpressions.Keys) {
				string fieldValue;
				if (postedData.TryGetString(fieldName, out fieldValue) && FieldExpressions [fieldName].IsMatch (fieldValue)) {
					int number;
					if (int.TryParse (fieldValue, out number)) {
						this [fieldName] = number;
					} else {
						if (HtmlEscape)
							fieldValue = HttpUtility.HtmlEncode(fieldValue);
						this[fieldName] = fieldValue;
					}
				} else {
					if (fieldDefaults.Has (fieldName))
						this [fieldName] = this.GetString (fieldName, "");

					FaultyFields.Add (fieldName);
				}
			}
		}
	}
}

