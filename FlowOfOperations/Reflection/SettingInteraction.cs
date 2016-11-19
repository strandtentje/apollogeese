using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	class SettingInteraction : SimpleInteraction
	{
		public SettingInteraction (IInteraction parameters, string Key, object Value) : base(parameters)
		{
			this ["settingkey"] = Key;
			this ["settingvalue"] = Value;
		}
	}
}

