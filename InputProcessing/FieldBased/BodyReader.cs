using System;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.ApolloGeese.Http;
using BorrehSoft.Utensils.Collections.Maps;
using System.Web;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.InputProcessing
{
	/// <summary>
	/// Reads field values from user input suffixed to the request URl, after the question mark (?)
	/// </summary>
	public class BodyReader : FieldReader
	{
		static char assigner = '=';
		static char concatenator = '&';

		public override Map<object> Deserialize (string data)
		{
			SerializingMap<object> postedData = new SerializingMap<object> ();

			if (data != null) postedData.AddFromString ( data, HttpUtility.UrlDecode, assigner, concatenator, -1);
			
			return postedData;
		}
	}
}
