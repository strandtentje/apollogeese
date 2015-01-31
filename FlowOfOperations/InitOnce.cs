using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.Extensions.BasicWeblings
{
	/// <summary>
	/// Service that branches downstream once, when all downstream branches
	/// have finished loading.
	/// </summary>
	public class InitOnce : Service
	{
		private Service begin;

		public override string Description {
			get {
				return "Runs attached branch upon startup";
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "begin") {
				begin = e.NewValue;
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			this.AllBrancesLoaded += HandleAllBrancesLoaded;
		}

		protected override bool Process (IInteraction parameters)
		{
			return begin.TryProcess (parameters);
		}

		void HandleAllBrancesLoaded (object sender, EventArgs e)
		{
			begin.TryProcess (new QuickInteraction ());
		}
	}
}

