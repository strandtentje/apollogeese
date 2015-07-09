using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace Testing
{
	/// <summary>
	/// Test context.
	/// </summary>
	class TestContext : QuickInteraction
	{
		/// <summary>
		/// Gets the name of the originating case.
		/// </summary>
		/// <value>The name of the originating case.</value>
		public string OriginatingCaseName {
			get;
			private set;
		}
		 
		public TestContext (string name, Settings availableContext) : base(null, availableContext)
		{
			this.OriginatingCaseName = name;
		}
	}
}

