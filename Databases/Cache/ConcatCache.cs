using System;
using BorrehSoft.ApolloGeese.Extensions.Data.Cache;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Collections.Generic;
using System.Text;

namespace Data
{
	public class ConcatCache : ContextCache
	{
		IEnumerable<string> CacheKeySources {
			get;
			set;
		}

		protected override void Initialize (Settings modSettings)
		{
			base.Initialize (modSettings);

			this.CacheKeySources = modSettings.GetStringList ("keynames", new string[] { this.CacheNameSource });
		}

		protected override string GetCacheName (IInteraction parameters)
		{
			StringBuilder keyBuilder = new StringBuilder ();

			foreach (string keySource in this.CacheKeySources) {
				object keyCandidate;
				if (parameters.TryGetFallback (keySource, out keyCandidate)) {
					keyBuilder.Append (keyCandidate.ToString());
				}
				keyBuilder.AppendLine ();
			}

			return keyBuilder.ToString ();
		}
	}
}

