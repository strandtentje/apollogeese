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
		/// <summary>
		/// Cached caches of module files.
		/// </summary>
		private static Map<CachedInstances> cache = new Map<CachedInstances>();

		/// <summary>
		/// Gets the service instances for a module file.
		/// </summary>
		/// <returns>The instances.</returns>
		/// <param name="file">File.</param>
		/// <param name="loadPlugins">If set to <c>true</c> load plugins.</param>
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

		/// <summary>
		/// Produces a new cache of instantiated services for a certain module file.
		/// </summary>
		/// <returns>The new instances.</returns>
		/// <param name="info">Info.</param>
		/// <param name="loadPlugins">If set to <c>true</c> load plugins.</param>
		private static CachedInstances GetNewInstances(FileInfo info, bool loadPlugins)
		{
			return new CachedInstances (
				(new Complinker (info.FullName, loadPlugins)).GetInstances (),
				info.LastWriteTime);
		}
	}
}

