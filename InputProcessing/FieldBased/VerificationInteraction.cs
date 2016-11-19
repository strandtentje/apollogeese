using System;
using System.Text;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Collections.Generic;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;
using System.Text.RegularExpressions;
using System.Web;
using BorrehSoft.Utilities.Collections;

namespace BorrehSoft.ApolloGeese.Extensions.InputProcessing
{
	/// <summary>
	/// Interaction that represents precisely one field verification action.
	/// Shouldn't go downstream unless error occured.
	/// </summary>
	class VerificationInteraction : SimpleInteraction
	{
		private string sourceName;

		public Dictionary<string, Regex> FieldExpressions { get; private set; }
		public List<string> FaultyFields { get; private set; }
		public List<string> FallbackNames { get; private set; }
		public bool HtmlEscape { get; set; }

		public VerificationInteraction (IInteraction parameters, string sourceName, Dictionary<string, Regex> fieldExpressions, List<string> fallbackNames) : base(parameters)
		{
			this.FieldExpressions = fieldExpressions;
			this.FallbackNames = fallbackNames;
			this.sourceName = sourceName;
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
		/// Gets the posted data.
		/// </summary>
		/// <value>The posted data.</value>
		private Map<object> PostedData { 
			get {
				return (Map<object>)this [sourceName];
			}
		}

		/// <summary>
		/// Loads the fields.
		/// </summary>
		/// <param name="postedData">Posted data.</param>
		/// <param name="fieldDefaults">Field defaults.</param>
		public void LoadFields(Map<string> fieldDefaults)
		{			
			FaultyFields = new List<string> ();

			// if you squint your eyes you can already see the bugs in this
			// code.
			foreach (string fieldName in FieldExpressions.Keys) {
				string fieldValue;
				if (PostedData.TryGetString(fieldName, out fieldValue)) {
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
						} else {
							FaultyFields.Add (fieldName);
						}
					}
				}
			}
		}
	}
}
