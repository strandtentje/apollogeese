using System;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using System.Web;

namespace ExternalData
{
	class InputInteraction<T> : NameValueInteraction
	{
		public InputInteraction (string name, T value, IInteraction parent) : base (parent)
		{
			this.Name = name;
			this.Value = value;
		}
	}

}

