using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Cache
{
	public class NosyCache : ContextCache
	{
		bool includeContext;

		protected override void Initialize (Settings modSettings)
		{
			includeContext = modSettings.GetBool ("includecontext", false);

			base.Initialize (modSettings);
		}

		protected override string GetCacheName (IInteraction parameters)
		{
			NosyInteraction interaction = new NosyInteraction (includeContext, parameters);
			string cacheName;

			if (BeginBranch.TryProcess (interaction)) {
				cacheName = interaction.Signature;
			} else {
				throw new Exception ("begin-branch of nosy interaction produced false.");
			}

			return cacheName;
		}
	}
}

