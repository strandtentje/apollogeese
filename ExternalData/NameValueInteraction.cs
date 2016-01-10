using System;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace ExternalData
{
	public class NameValueInteraction : SimpleInteraction
	{
		public string Name {
			get { return this ["name"]; }
			set { this ["name"] = value; }
		}

		public string Value {
			get { return this ["value"]; }
			set { this ["value"] = value; }
		}
	}
}

