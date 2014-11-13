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
	public abstract class FieldReader : Service
	{
		public override string Description {
			get {
				if (FieldExpressions == null)
					return "None";
				else 
					return string.Format(string.Join(",", FieldExpressions.Keys));
			}
		}		

		public Dictionary<string, Regex> FieldExpressions { get; private set; }
		private Service Succesful, Form;
		bool htmlEscape;

		protected override void Initialize (Settings modSettings)
		{
			Settings fieldRegexes = modSettings.Get ("fieldregexes", new object ()) as Settings;
			htmlEscape = modSettings.GetBool("escapehtml", true);

			if (fieldRegexes == null) {
				throw new Exception ("Service requires fieldregexes to be assigned a block of assingments.");
			} else {
				FieldExpressions = new Dictionary<string, Regex> ();
				foreach (string fieldName in fieldRegexes.Dictionary.Keys) {
					FieldExpressions.Add (fieldName, new Regex (fieldRegexes [fieldName] as string));
				}
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "successful") Succesful = e.NewValue;
			if (e.Name == "form") Form = e.NewValue;
		}

		public virtual string AcquireData (IInteraction parameters)
		{
			IIncomingBodiedInteraction request;
			request = (IIncomingBodiedInteraction)parameters.GetClosest(typeof(IIncomingBodiedInteraction));

			return request.IncomingBody.ReadToEnd ();
		}

		public abstract Map<object> Deserialize(string data);

		protected override bool Process (IInteraction parameters)
		{
			int number, failures = 0;
			bool success = true;

			QuickInteraction parsedData = new QuickInteraction (parameters);

			Map<object> postedData = Deserialize (AcquireData(parameters));

			foreach (string fieldName in FieldExpressions.Keys) {
				string fieldValue = postedData.GetString (fieldName, "");
				if (FieldExpressions [fieldName].IsMatch (fieldValue)) {
					if (int.TryParse (fieldValue, out number)) {
						parsedData [fieldName] = number;
					} else {
						if (htmlEscape)
							fieldValue = HttpUtility.HtmlEncode(fieldValue);
						parsedData[fieldName] = fieldValue;
					}
				} else {
					failures++;
					string failName = string.Format ("{0}_failure", fieldName);
					if (Branches.Has(failName)) {
						Service failBranch = Branches [failName];
						success &= failBranch.TryProcess (parameters);
					}
				}
			}

			if (failures == 0) {
				success &= Succesful.TryProcess (parsedData);
			} else {
				success &= Form.TryProcess(parsedData);
			}

			return success;
		}

	}
}

