using System;
using System.Text;
using BorrehSoft.ApolloGeese.Duckling;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.Http;
using System.Web;
using BorrehSoft.Utensils.Collections;

namespace BorrehSoft.Extensions.BasicWeblings
{
	class VerificationInteraction : QuickInteraction
	{
		public Dictionary<string, Regex> FieldExpressions { get; private set; }
		public List<string> FaultyFields { get; private set; }
		public List<string> FallbackNames { get; private set; }
		public bool HtmlEscape { get; set; }

		public VerificationInteraction (IInteraction parameters, Dictionary<string, Regex> fieldExpressions, List<string> fallbackNames) : base(parameters)
		{
			this.FieldExpressions = fieldExpressions;
			this.FallbackNames = fallbackNames;
		}

		private void ParseLoad(string fieldName, string fieldValue)
		{
			int number;
			if (int.TryParse (fieldValue, out number)) {
				this [fieldName] = number;
			} else {
				if (HtmlEscape)
					fieldValue = HttpUtility.HtmlEncode (fieldValue);
				this [fieldName] = fieldValue;
			}
		}

		/// <summary>
		/// Loads the fields.
		/// </summary>
		/// <param name="postedData">Posted data.</param>
		/// <param name="fieldDefaults">Field defaults.</param>
		public void LoadFields(Map<object> postedData, Map<string> fieldDefaults)
		{			
			FaultyFields = new List<string> ();

			// if you squint your eyes you can already see the bugs in this
			// code.
			foreach (string fieldName in FieldExpressions.Keys) {
				string fieldValue;
				if (postedData.TryGetString(fieldName, out fieldValue)) {
					if (FieldExpressions [fieldName].IsMatch (fieldValue)) {
						ParseLoad (fieldName, fieldValue);
					} else {
						if (fieldDefaults.TryGetString (fieldName, out fieldValue)) {
							this [fieldName] = fieldValue;
						}							

						FaultyFields.Add (fieldName);
					}
				} else {
					object fallbackObject;
					if (FallbackNames.Contains(fieldName) && this.TryGetFallback (fieldName, out fallbackObject)) {
						this [fieldName] = fallbackObject;
					} else {
						if (fieldDefaults.TryGetString (fieldName, out fieldValue)) {
							this [fieldName] = fieldValue;
						}		

						FaultyFields.Add (fieldName);
					}
				}
			}
		}
	}
}
