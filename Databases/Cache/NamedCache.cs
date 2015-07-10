using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Cache
{
	/// <summary>
	/// Named cache; has multiple caches by names. Useful when using the same Cache-instance for different but
	/// not too many types of datastreams.
	/// </summary>
	public class NamedCache : Service
	{
		public override string Description {
			get {
				return "Named Cache";
			}
		}

		Map<Cache> cacheMap = new Map<Cache>();

		string CacheNameSource {
			get;
			set;
		}

		Service BeginBranch {
			get;
			set;
		}

		protected override void Initialize (Settings modSettings)
		{
			this.CacheNameSource = modSettings.GetString ("cachenamesource", "cachename");
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "begin") {
				this.BeginBranch = e.NewValue;
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			string cacheName;

			if (!parameters.TryGetString (this.CacheNameSource, out cacheName))
				cacheName = "noname";

			Cache currentCache;

			if (cacheMap.Has (cacheName))
				currentCache = cacheMap [cacheName];
			else 
				currentCache = cacheMap [cacheName] = new Cache ();

			return currentCache.TryProcess (parameters);
		}
	}
}
