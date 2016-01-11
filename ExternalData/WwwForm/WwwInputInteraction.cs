	using System;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using System.Web;

namespace ExternalData
{
	class WwwInputInteraction : NameValueInteraction
	{
		public WwwInputInteraction (string pair, IInteraction parameters) : base(parameters)
		{
			string[] splitPair = pair.Split ('=');

			this.Name = splitPair [0];
			if (splitPair.Length > 1)
				this.Value = HttpUtility.UrlDecode (splitPair [1]);
			else
				this.Value = "";
		}
	}

}

