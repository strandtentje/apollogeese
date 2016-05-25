using System;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Log;

namespace ExternalData
{
	public class MultipartForm : NameValueService
	{
		public override string Description {
			get {
				return "Multipart Form parser";
			}
		}

	}
}

