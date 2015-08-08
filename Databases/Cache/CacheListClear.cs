using System;
using System.Collections.Generic;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Cache
{
	public class CacheListClear : CacheListItem
	{
		public override string Description {
			get {
				return string.Format ("clearing of list");
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}

		bool purge;

		protected override void Initialize (Settings modSettings)
		{
			base.Initialize (modSettings);

			purge = modSettings.GetBool ("purge", false);
		}

		protected override bool Process (IInteraction parameters)
		{
			CacheInteraction cache;
			cache = (CacheInteraction)parameters.GetClosest (typeof(CacheInteraction));

			if (purge)
				cache.Purge ();
			else 
				cache.Clear ();

			return true;
		}
	}
}
