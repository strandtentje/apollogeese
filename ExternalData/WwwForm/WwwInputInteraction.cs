using System;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;

namespace ExternalData
{
	class WwwInputInteraction : SimpleInteraction
	{
		public WwwInputInteraction (string pair, IInteraction parameters) : base(parameters)
		{
			string[] splitPair = pair.Split ('=');

			this.Name = splitPair [0];
			if (splitPair.Length > 1)
				this.Value = splitPair [1];
			else
				this.Value = "";

			this ["name"] = this.Name;
			this ["value"] = this.Value;
		}

		public string Name {
			get;
			set;
		}

		public string Value {
			get;
			set;
		}
	}

}

