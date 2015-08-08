using System;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	/// <summary>
	/// A stub-service; exists once, does nothing. Intended to cap off unattached branches.
	/// </summary>
	public class StubService : Service
	{
		static StubService instance;

		public StubService() {}

		public override string Description { get { return "Control flows back upstream"; } }

		protected override void Initialize (Settings modSettings){ }

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}

        public override bool FastProcess(IFast parameter)
        {
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

