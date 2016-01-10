using System;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;

namespace ExternalData
{
	class WwwFormInteraction : SimpleInteraction
	{
		public WwwFormInteraction (IInteraction parameters) : base(parameters)
		{

		}

		public void Register (WwwInputInteraction input)
		{
			this [input.Name] = input.Value;
		}
	}

}

