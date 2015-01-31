using System;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.ApolloGeese.Duckling;
using System.IO;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.ApolloGeese.Loader
{
	/// <summary>
	/// Instance loader; loads instances from files.
	/// </summary>
	public static class InstanceLoader
	{
		private static Map<CachedInstances> cache = new Map<CachedInstances>();

		public static Map <Service> GetInstances(string file, bool loadPlugins = false)
		{
			FileInfo info = new FileInfo (file);

			CachedInstances entry, existing = cache [file];

			if (existing == null) {
				entry = GetNewInstances (info, loadPlugins);
				cache [file] = entry;
			} else {
				if (existing.LastChanged.Equals (info.LastWriteTime)) {
					entry = existing;
				} else {
					Secretary.Report (5, "Outdated instances in file: ", info.Name);
					entry = GetNewInstances (info, loadPlugins);
					cache [file] = entry;
					existing.Dispose ();
				}
			}

			return entry.Instances;
		}

		private static CachedInstances GetNewInstances(FileInfo info, bool loadPlugins)
		{
			return new CachedInstances (
				(new Complinker (info.FullName, loadPlugins)).GetInstances (),
				info.LastWriteTime);
		}
	}
}

