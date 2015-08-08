using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Collections.Generic;

namespace Testing
{
	/// <summary>
	/// Test context.
	/// </summary>
	class TestContextInteraction : QuickInteraction
	{
		public List<ProbeResultInteraction> ProbeResults {
			get;
			private set;
		}

		/// <summary>
		/// Gets the name of the originating case.
		/// </summary>
		/// <value>The name of the originating case.</value>
		public string OriginatingCaseName {
			get;
			private set;
		}
		 
		public TestContextInteraction (string name, Settings availableContext) : base(null, availableContext)
		{
			this.ProbeResults = new List<ProbeResultInteraction> ();
			this.OriginatingCaseName = name;
		}
	}
}

