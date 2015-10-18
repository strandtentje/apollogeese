using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.IO;
using System.Collections.Generic;
using BorrehSoft.Utensils.Log;

namespace InputProcessing
{
	class KeyValueInteraction : SimpleInteraction
	{
		public KeyValueInteraction (IInteraction parent, string name, object value) : base(parent)
		{
			this ["name"] = name; this ["value"] = value;
		}
	}
}
