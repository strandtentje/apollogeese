using System;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace ExternalData
{
	public class NameValueInteraction : SimpleInteraction
	{
		public NameValueInteraction (IInteraction parent) : base(parent){}

		public string Name {
			get { return this.GetString("name"); }
			set { this ["name"] = value; }
		}

		public object Value {
			get { return this["value"]; }
			set { this ["value"] = value; }
		}
	}
}

