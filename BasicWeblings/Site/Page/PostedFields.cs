using System;
using System.Text;
using BorrehSoft.ApolloGeese.Duckling;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.Duckling.Http;
using System.Web;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class PostedFields : Service
	{
		public override string Description {
			get {
				return string.Format("Registration of fields {0}", string.Join(",", FieldExpressions.Keys));
			}
		}		

		public Dictionary<string, Regex> FieldExpressions { get; private set; }

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

		protected override bool Process (IInteraction parameters)
		{
			IHttpInteraction request = parameters as IHttpInteraction;
			SerializingMap<object> postedData = new SerializingMap<object>();

			postedData.AddFromString(request.RequestBody.ReadToEnd(), HttpUtility.UrlDecode, '=', '&');



			return true;
		}

	}
}

