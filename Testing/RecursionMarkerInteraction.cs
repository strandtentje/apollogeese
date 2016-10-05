using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;

namespace Testing
{
	class RecursionMarkerInteraction : SimpleInteraction
	{
		public HaltRecursion Placer { get; private set; }

		public RecursionMarkerInteraction (HaltRecursion haltRecursion)
		{
			this.Placer = haltRecursion;
		}
	}
}

