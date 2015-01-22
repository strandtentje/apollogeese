using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Duckling.Loader
{
	public class CachedInstances : IDisposable
	{
		public IEnumerable<Service> Instances { get; private set; }

		public DateTime LastChanged { get; private set; }

		public CachedInstances(IEnumerable<Service> instances, DateTime lastChanged)
		{
			this.Instances = instances;
			this.LastChanged = lastChanged;
		}

		public void Dispose()
		{
			foreach (Service service in Instances)
				service.Dispose ();
		}
	}
}

