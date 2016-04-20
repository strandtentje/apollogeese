	using System;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using System.Web;

namespace ExternalData
{
	class WwwInputInteraction : NameValueInteraction
	{
		public static WwwInputInteraction FromPair (string pair, IInteraction parameters)
		{
			string[] splitPair = pair.Split ('=');

			string name = splitPair [0];
			string value = "";
			if (splitPair.Length > 1)
				value = HttpUtility.UrlDecode (splitPair [1]);	

			return new WwwInputInteraction (name, value, parameters);
		}
			
		public WwwInputInteraction (string name, string value, IInteraction parent) : base (parent) 
		{
			this.Name = name;
			this.Value = value;
		}
	}

}

