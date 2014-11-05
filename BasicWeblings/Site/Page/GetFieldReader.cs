using System;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.ApolloGeese.Duckling.Http;
using BorrehSoft.Utensils.Collections.Maps;
using System.Web;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page
{
	public class GetFieldReader : FieldReader
	{
		public override string AcquireData (IInteraction parameters)
		{
			IHttpInteraction request = (IHttpInteraction)parameters.GetClosest(typeof(IHttpInteraction));

			return request.GetQuery;
		}

		public override Map<object> Deserialize (string data)
		{
			SerializingMap<object> postedData = new SerializingMap<object> ();

			if (data != null) postedData.AddFromString (data, HttpUtility.UrlDecode, '=', '&');
			
			return postedData;
		}
	}
}

