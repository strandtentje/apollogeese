using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections;

namespace InputProcessing
{
	class SplitInteraction :SimpleInteraction
	{
		public SplitInteraction (int currentItem, IInteraction parameters, string to, string split) : base(parameters)
		{
			this [to] = split;
			this ["index"] = currentItem;
		}
	}
}

