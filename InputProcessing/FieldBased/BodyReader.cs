using System;
using BorrehSoft.Utilities.Collections;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using System.Web;
using BorrehSoft.Utilities.Collections.Settings;

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

