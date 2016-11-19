using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections;

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

