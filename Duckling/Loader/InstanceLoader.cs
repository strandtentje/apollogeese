using System;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.ApolloGeese.Duckling;
using System.IO;

namespace BorrehSoft.ApolloGeese.Duckling.Loader
{
	/// <summary>
	/// Instance loader; loads instances from files.
	/// </summary>
	public static class InstanceLoader
	{
		private static Map<CachedInstances> cache = new Map<CachedInstances>();

		public static IEnumerable <Service> GetInstances(string file)
		{
			FileInfo info = new FileInfo (file);

			CachedInstances entry, existing = cache [file];

			if (existing == null) {
				entry = GetNewInstances (info);
			} else {
				if (existing.LastChanged.Equals (info.LastWriteTime)) {
					entry = existing;
				} else {
					entry = GetNewInstances (info);
					cache [file] = entry;
					existing.Dispose ();
				}
			}

			return entry.Instances;
		}

		private static CachedInstances GetNewInstances(FileInfo info)
		{
			return new CachedInstances (
				(new Complinker (info.FullName)).GetInstances (),
				info.LastWriteTime);
		}
	}
}

