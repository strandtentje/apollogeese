using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Cache
{
	/// <summary>
	/// Named cache; has multiple caches by names. Useful when using the same Cache-instance for different but
	/// not too many types of datastreams.
	public class ContextCache : SingleBranchService
	/// </summary>
	{
		public override string Description {
			get {
				return "Named Cache";
			}
		}

		Map<AnonymousCache> cacheMap = new Map<AnonymousCache>();

		Settings cacheSettings = new Settings();

		protected string CacheNameSource {
			get;
			set;
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			this.Settings ["keyname"] = defaultParameter;
		}

		bool Reset {
			get;
			set;
		}

		protected override void Initialize (Settings modSettings)
		{
			this.CacheNameSource = modSettings.GetString ("keyname", "cachename");
			this.Reset = modSettings.GetBool ("reset", false);

			if (modSettings.Has ("lifetime"))
				cacheSettings ["lifetime"] = modSettings ["lifetime"];
		}

		protected virtual string GetCacheName(IInteraction parameters) {
			string cacheName;

			if (!parameters.TryGetFallbackString (this.CacheNameSource, out cacheName))
				cacheName = "noname";

			return cacheName;
		}

		protected override bool Process (IInteraction parameters)
		{
			string cacheName = GetCacheName (parameters);

			AnonymousCache currentCache;

			if (this.Reset) {
				cacheMap.Dictionary.Remove (cacheName);
			} 

			if (cacheMap.Has (cacheName)) {
				currentCache = cacheMap [cacheName];
			} else {
				currentCache = cacheMap [cacheName] = new AnonymousCache ();
				currentCache.SetSettings (cacheSettings);
				currentCache.Branches [SingleBranchNames.With] = this.WithBranch;
			}

			return currentCache.TryProcess (parameters);
		}
	}
}
