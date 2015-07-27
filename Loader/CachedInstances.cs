using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections;

namespace BorrehSoft.ApolloGeese.Loader
{
	/// <summary>
	/// Cached Service instances for single module.
	/// </summary>
	public class CachedInstances : IDisposable
	{
		/// <summary>
		/// Gets the instances by name
		/// </summary>
		/// <value>The instances.</value>
		public Map<Service> Instances { get; private set; }

		/// <summary>
		/// Gets the Date and Time whereon the file was last changed
		/// </summary>
		/// <value>The last changed date.</value>
		public DateTime LastChanged { get; private set; }

		/// <summary>
		/// Gets the meta data.
		/// </summary>
		/// <value>The meta data.</value>
		public Map<object> MetaData { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.Loader.CachedInstances"/> class.
		/// </summary>
		/// <param name="instances">Instances.</param>
		/// <param name="lastChanged">Last changed.</param>
		public CachedInstances(Map<Service> instances, Map<object> metaData, DateTime lastChanged)
		{
			this.Instances = instances;
			this.MetaData = metaData;
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
			foreach (Service service in Instances.Dictionary.Values)
				service.Dispose ();
		}
	}
}

