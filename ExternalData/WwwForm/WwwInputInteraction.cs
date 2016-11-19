	using System;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using System.Web;

namespace ExternalData
{
	class WwwInputInteraction : NameValueInteraction
	{
		public WwwInputInteraction (string name, string value, IInteraction parent) : base (parent) 
		{
			this.Name = name;
			this.Value = value;
		}
	}

}

