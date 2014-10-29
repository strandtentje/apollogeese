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
				return string.Format("Registration of fields {0}", string.Join(",", FieldExpressions.Keys));
			}
		}		

		public Dictionary<string, Regex> FieldExpressions { get; private set; }
		private Service Succesful;

		protected override void Initialize (Settings modSettings)
		{
			Settings fieldRegexes = modSettings.Get("fieldregexes", new object()) as Settings;

			if (fieldRegexes == null) {
				throw new Exception("Service requires fieldregexes to be assigned a block of assingments.");
			} else {
				FieldExpressions = new Dictionary<string, Regex>();
				foreach(string fieldName in fieldRegexes.Dictionary.Keys)
				{
					FieldExpressions.Add(fieldName, new Regex(fieldRegexes[fieldName] as string));
				}
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "successful") Succesful = e.NewValue;
		}

		public abstract Map<object> Deserialize(string data);

		protected override bool Process (IInteraction parameters)
		{
			int number, failures = 0;
			bool success = true;

			IIncomingBodiedInteraction request = parameters as IIncomingBodiedInteraction;
			QuickInteraction parsedData = new QuickInteraction ();

			Map<object> postedData = Deserialize(request.IncomingBody.ReadToEnd ());

			foreach (string fieldName in FieldExpressions.Keys) {
				string fieldValue = postedData.GetString (fieldName, "");
				if (FieldExpressions [fieldName].IsMatch (fieldValue)) {
					if (int.TryParse (fieldValue, out number)) {
						parsedData [fieldName] = number;
					} else {
						parsedData [fieldName] = fieldValue;
					}
				} else {
					failures++;
					success &= Branches [string.Format ("%s_failure", fieldName)].TryProcess (parameters);
				}
			}

			if (failures == 0) {
				success &= Succesful.TryProcess(parsedData);
			}

			return success;
		}

	}
}

