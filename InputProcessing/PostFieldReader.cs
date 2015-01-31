using System;
using BorrehSoft.Utensils.Collections.Maps;
using System.Web;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.ApolloGeese.Duckling;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page
{
	public class PostFieldReader : FieldReader
	{
		public override Map<object> Deserialize (string data)
		{			
				SerializingMap<object> postedData = new SerializingMap<object> ();
				postedData.AddFromString (data, HttpUtility.UrlDecode, '=', '&');

				return postedData;
		}
	}
}

