using System;
using BorrehSoft.Utensils.Collections.Maps;
using System.Web;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.ApolloGeese.Duckling;

namespace BorrehSoft.ApolloGeese.Extensions.InputProcessing
{
	/// <summary>
	/// Reads fields from POST-body into context.
	/// </summary>
	public class PostFieldReader : FieldReader
	{
		public override Map<object> Deserialize (string data)
		{			
			SerializingMap<object> postedData = new SerializingMap<object> ();
			postedData.AddFromString (data, HttpUtility.UrlDecode, '=', '&');

			return postedData;
		}

		protected override string SourceName {
			get {
				return "postfields";
			}
		}
	}
}

