using System;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Duckling
{
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
			Secretary.Report (5, "Warning: Succesfully did nothing.", parameters.ToString());
			return true; 
		}

		public static Service Instance {
			get {
				instance = instance ?? new StubService();
				return instance;
			}
		}
	}
}

