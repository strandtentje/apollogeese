using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Cache
{
	/// <summary>
	/// Cache list item.
	/// </summary>
	public class CacheListItem : Service
	{
		public override string Description {
			get {
				return string.Format ("item of list");
			}
		}

		protected override void Initialize (Settings modSettings)
		{

		}

		protected override bool Process (IInteraction parameters)
		{
			CacheInteraction cache;
			cache = (CacheInteraction)parameters.GetClosest (typeof(CacheInteraction));

			cache.List.Add (parameters);

			return true;
		}
	}
}

