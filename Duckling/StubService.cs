using System;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.ApolloGeese.Duckling
{
	class StubService : Service
	{
		static StubService instance;

		private StubService() {}

		public override string[] AdvertisedBranches { get { return new string[] {}; } }

		public override string Description { get { return "Nothing here."; } }

		protected override void Initialize (Settings modSettings){ }

		protected override bool Process (IInteraction parameters) { 
			Secretary.Report (5, "Warning: Stub hit.");
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

