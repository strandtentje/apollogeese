using System;
using BorrehSoft.ApolloGeese.Duckling;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class IterateInteraction : QuickInteraction
	{
		public IterateInteraction (IInteraction parameters, string iterateName, string iterateValue) : base(parameters)
		{
			this[iterateName] = iterateValue;
		}
	}
}

