using System;
using System.IO;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace Testing
{
	class ProbeResult : QuickInteraction
	{
		public ProbeResult(IInteraction parent, string key, object referenceValue, object foundValue) : base(Parent) {
			this ["probekey"] = key;
			this ["refvalue"] = referenceValue;
			this ["foundvalue"] = foundValue;
		}
	}
}

