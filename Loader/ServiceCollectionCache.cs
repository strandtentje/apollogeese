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
		/// Get ServiceCollection for specified file, loadPlugins and loadBinPlugins.
		/// </summary>
		/// <param name="file">File.</param>
		/// <param name="loadPlugins">If set to <c>true</c> load plugins.</param>
		/// <param name="loadBinPlugins">If set to <c>true</c> load bin plugins.</param>
		public static ServiceCollection Get(
			string file,
			bool loadPlugins = false,
			bool loadBinPlugins = false
		) {
			return Get (file, (new FileInfo (file)).DirectoryName, loadPlugins, loadBinPlugins);
		}

		/// <summary>
		/// Gets the service instances for a module file.
		/// </summary>
		/// <returns>The instances.</returns>
		/// <param name="file">File.</param>
		/// <param name="loadPlugins">If set to <c>true</c> load plugins.</param>
		public static ServiceCollection Get(
			string filePath, 
			string workingDirectory,
			bool loadPlugins = false, 
			bool loadBinPlugins = false
		) {
			FileInfo info = new FileInfo (filePath);

			string cacheKey = string.Format ("{0}|*|{1}", info.FullName, workingDirectory);

			ServiceCollection resultCollection = null;
			ServiceCollection cachedCollection = cache.Get(cacheKey, null);
			bool wasCollectionCached = cachedCollection != null;
			bool cacheValid = wasCollectionCached && cachedCollection.LastChanged.Equals (info.LastWriteTime);

			if (cacheValid) {				
				resultCollection = cachedCollection;
			} else {
				if (wasCollectionCached) {
					Secretary.Report (
						5, "Disposing outdated ServiceCollection", 
						cachedCollection.ToString ());
					cachedCollection.Dispose ();
				}

				resultCollection = ServiceCollection.CreateFromFileForDirectory(
					info, workingDirectory, loadPlugins,  loadBinPlugins);
				Secretary.Report (
					5, "Instantiated new ServiceCollection", 
					resultCollection.ToString());
				cache [cacheKey] = resultCollection;
			}

			return resultCollection;
		}
	}
}

