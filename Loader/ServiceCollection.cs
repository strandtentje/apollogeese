using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections;
using System.IO;
using System.Reflection;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.ApolloGeese.Loader
{
	/// <summary>
	/// Cached Service instances for single module.
	/// </summary>
	public class ServiceCollection : Map<Service>
	{
		/// <summary>
		/// Gets the Date and Time whereon the file was last changed
		/// </summary>
		/// <value>The last changed date.</value>
		public DateTime LastChanged { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.Loader.CachedInstances"/> class.
		/// </summary>
		/// <param name="instances">Instances.</param>
		/// <param name="lastChanged">Last changed.</param>
		public ServiceCollection(Map<Service> services, DateTime lastChanged) : base(services)
		{
			this.LastChanged = lastChanged;
		}

		/// <summary>
		/// Releases all resource used by the <see cref="BorrehSoft.ApolloGeese.Loader.CachedInstances"/> object, 
		/// including underlying services.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the
		/// <see cref="BorrehSoft.ApolloGeese.Loader.CachedInstances"/>. The <see cref="Dispose"/> method leaves the
		/// <see cref="BorrehSoft.ApolloGeese.Loader.CachedInstances"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the
		/// <see cref="BorrehSoft.ApolloGeese.Loader.CachedInstances"/> so the garbage collector can reclaim the memory that
		/// the <see cref="BorrehSoft.ApolloGeese.Loader.CachedInstances"/> was occupying.</remarks>
		public void Dispose()
		{
			foreach (Service service in this.Dictionary.Values)
				service.Dispose ();
		}

		/// <summary>
		/// Creates from complinker.
		/// </summary>
		/// <returns>The from complinker.</returns>
		/// <param name="complinker">Complinker.</param>
		/// <param name="loadPlugins">If set to <c>true</c> load plugins.</param>
		/// <param name="loadBinPlugins">If set to <c>true</c> load bin plugins.</param>
		public static ServiceCollection CreateFromComplinker(Complinker complinker, bool loadPlugins, bool loadBinPlugins = false)
		{
			if (loadPlugins)
				complinker.LoadPlugins ();

			if (loadBinPlugins) {
				DirectoryInfo binDirectory = new DirectoryInfo (
					Assembly.GetExecutingAssembly ().Location);
				Secretary.Report (0, 
					"Bin directory: ", binDirectory.FullName);
				complinker.ScanDirectoryForPlugins (
					binDirectory.Parent.FullName);
			}

			return new ServiceCollection (
				complinker.GetInstances (), 
				complinker.ConfigFile.LastWriteTime);
		}

		/// <summary>
		/// Creates InstanceCache from file.
		/// </summary>
		/// <returns>The from file.</returns>
		/// <param name="info">Info.</param>
		/// <param name="loadPlugins">If set to <c>true</c> load plugins.</param>
		/// <param name="loadBinPlugins">If set to <c>true</c> load bin plugins.</param>
		public static ServiceCollection CreateFromFile(FileInfo info, bool loadPlugins, bool loadBinPlugins = false) 
		{
			return CreateFromComplinker (
				new Complinker (info), 
				loadPlugins, 
				loadBinPlugins);
		}

		public override string ToString ()
		{
			return string.Format ("[ServiceCollection: LastChanged={0}]", LastChanged);
		}
	}
}

