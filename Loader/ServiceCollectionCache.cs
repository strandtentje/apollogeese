using System;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using BorrehSoft.Utensils.Log;
using System.Reflection;

namespace BorrehSoft.ApolloGeese.Loader
{
	/// <summary>
	/// Instance loader; loads instances from files.
	/// </summary>
	public static class ServiceCollectionCache
	{
		private static Map<ServiceCollection> cache = new Map<ServiceCollection>();

		/// <summary>
		/// Gets the service instances for a module file.
		/// </summary>
		/// <returns>The instances.</returns>
		/// <param name="file">File.</param>
		/// <param name="loadPlugins">If set to <c>true</c> load plugins.</param>
		public static ServiceCollection Get(
			string file, 
			bool loadPlugins = false, 
			bool loadBinPlugins = false
		) {
			FileInfo info = new FileInfo (file);

			bool wasCollectionInCache = cache.Has (file);
			ServiceCollection cachedCollection = null;
			ServiceCollection resultCollection = null;

			if (wasCollectionInCache && cache.Get(file).Equals(info.LastWriteTime)) {				
				cachedCollection = resultCollection = cache.Get (file);
				Secretary.Report (
					5, 
					"Retrieved cached ServiceCollection", 
					cachedCollection.ToString());
			} else {
				resultCollection = ServiceCollection.CreateFromFile(
					info, 
					loadPlugins, 
					loadBinPlugins);
				Secretary.Report (
					5, 
					"Instantiated new ServiceCollection", 
					resultCollection.ToString());
				cache [file] = resultCollection;
			}

			if (wasCollectionInCache) {
				Secretary.Report (
					5, 
					"Disposing outdated ServiceCollection", 
					cachedCollection.ToString());
				cachedCollection.Dispose ();
			}

			return resultCollection;
		}
	}
}

