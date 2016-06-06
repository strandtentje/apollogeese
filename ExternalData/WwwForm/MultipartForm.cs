using System;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Log;
using System.IO;
using Parsing;

namespace ExternalData
{
	public class MultipartForm : HttpForm<byte[]>
	{
		public override string Description {
			get {
				return "Multipart Form parser";
			}
		}

		public override bool CheckMimetype (string mimeType)
		{
			return mimeType.StartsWith ("multipart/form-data; boundary=");
		}

		protected override void UrlParseReader (
			TextReader reader, 
			NameValuePiper<TextReader, byte[]>.NameValueCallback callback)
		{
			
		}
	}
}

