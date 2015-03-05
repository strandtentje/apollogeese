using System;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Duckling
{
	/// <summary>
	/// A stub-service; exists once, does nothing. Intended to cap off unattached branches.
	/// </summary>
	class StubService : Service
	{
		static StubService instance;

		public StubService() {}

		public override string Description { get { return "Nothing here."; } }

		protected override void Initialize (Settings modSettings){ }

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}

		protected override bool Process (IInteraction parameters) { 
			// Secretary.Report (5, "Warning: Succesfully did nothing.", parameters.ToString());
			return true; 
		}

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static Service Instance {
			get {
				instance = instance ?? new StubService();
				return instance;
			}
		}
	}
}

