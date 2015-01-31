using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections;

namespace BorrehSoft.ApolloGeese.Loader
{
	public class CachedInstances : IDisposable
	{
		public Map<Service> Instances { get; private set; }

		public DateTime LastChanged { get; private set; }

		public CachedInstances(Map<Service> instances, DateTime lastChanged)
		{
			this.Instances = instances;
			this.LastChanged = lastChanged;
		}

		public void Dispose()
		{
			foreach (Service service in Instances.Dictionary.Values)
				service.Dispose ();
		}
	}
}

