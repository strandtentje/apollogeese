using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;

namespace Testing
{
	class RecursionMarkerInteraction : SimpleInteraction
	{
		public RecursionMarkerInteraction (IInteraction parameters, HaltRecursion haltRecursion): base(parameters)
		{
			this.Placer = haltRecursion;
		}

		public HaltRecursion Placer { get; private set; }
	}
}

