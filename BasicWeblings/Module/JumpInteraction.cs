using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class JumpInteraction : QuickInteraction
	{
		public Map<Service> Branches { get; private set; }


		public JumpInteraction(IInteraction parent, Map<Service> branches, Settings modSettings) : base(parent, modSettings)
		{
			this.Branches = branches;

			object reassignObject = modSettings["reassignments"];

			if (reassignObject != null) {
				Settings reassignments = (Settings)reassignObject;
				object value;
				foreach (KeyValuePair<string, object> pair in reassignments.Dictionary) {
					string sourceName = pair.Value as string;
					if (parent.TryGetFallback (sourceName, out value)) 
						this [pair.Key] = value;
				}						
			}
		}
	}
}
