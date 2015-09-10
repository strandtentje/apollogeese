using System;
using System.IO;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace Testing
{
	class ProbeResultInteraction : SimpleInteraction
	{
		public ProbeResultInteraction(IInteraction parent, string probename, string key, object referenceValue, object foundValue) : base(parent) {
			this ["probename"] = probename;
			this ["probekey"] = key;
			this ["refvalue"] = referenceValue;
			this ["foundvalue"] = foundValue;
			this.IsMatch = referenceValue.Equals (foundValue);
		}

		public bool IsMatch {
			get;
			private set;
		}
	}
}

