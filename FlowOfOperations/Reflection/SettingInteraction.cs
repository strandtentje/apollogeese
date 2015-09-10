using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	class SettingInteraction : SimpleInteraction
	{
		public SettingInteraction (IInteraction parameters, string Key, object Value) : base(parameters)
		{
			this ["key"] = Key;
			this ["value"] = Value;
		}
	}
}

