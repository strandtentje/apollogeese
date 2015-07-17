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
	public class ContextCache : Service
	/// </summary>
	{
		public override string Description {
			get {
				return "Named Cache";
			}
		}

		Map<Cache> cacheMap = new Map<Cache>();

		Settings cacheSettings = new Settings();

		string CacheNameSource {
			get;
			set;
		}

		protected Service BeginBranch {
			get;
			set;
		}

		protected override void Initialize (Settings modSettings)
		{
			if (modSettings.Has ("default")) {
				CacheNameSource = (string)modSettings ["default"];
			} else if (modSettings.Has ("keyname")) {
				CacheNameSource = (string)modSettings ["keyname"];
			} else {
				CacheNameSource = "cachename";
			}

			if (modSettings.Has ("lifetime"))
				cacheSettings ["lifetime"] = modSettings ["lifetime"];
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "begin") {
				this.BeginBranch = e.NewValue;
			}
		}

		protected virtual string GetCacheName(IInteraction parameters) {
			string cacheName;

			if (!parameters.TryGetString (this.CacheNameSource, out cacheName))
				cacheName = "noname";

			return cacheName;
		}

		protected override bool Process (IInteraction parameters)
		{
			string cacheName = GetCacheName (parameters);

			Cache currentCache;

			if (cacheMap.Has (cacheName)) {
				currentCache = cacheMap [cacheName];
			} else {
				currentCache = cacheMap [cacheName] = new Cache ();
				currentCache.SetSettings (cacheSettings);
				currentCache.Branches ["begin"] = this.BeginBranch;
			}

			return currentCache.TryProcess (parameters);
		}
	}
}
