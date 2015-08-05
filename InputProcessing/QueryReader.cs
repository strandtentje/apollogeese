using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.ApolloGeese.Http;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Collections.Maps;
using System.Web;

namespace BorrehSoft.ApolloGeese.Extensions.InputProcessing
{
	public class QueryReader : FieldReader
	{		
		public override Map<object> GetParseableInput (IInteraction parameters)
		{
			IInteraction httpParam;
			Map<object> input;

			if (parameters.TryGetClosest (typeof(IHttpInteraction), out httpParam)) {
				IHttpInteraction httpInteraction = (IHttpInteraction)httpParam;

				SerializingMap<object> parsedQuery = new SerializingMap<object> ();

				parsedQuery.AddFromString (httpInteraction.GetQuery,
					HttpUtility.UrlDecode, '=', '&');

				input = parsedQuery;
			} else {
				throw new Exception ("Now http interaction found to get url query from.");
			}

			return input;
		}
	}
}

