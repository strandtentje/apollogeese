using System;
using System.Net;
using System.Collections.Generic;
using BorrehSoft.Utensils.Settings;

namespace BorrehSoft.ApolloGeese.Duckling
{
	public abstract class Service	
	{
		private Dictionary<string, ProcessCallback> branches = new Dictionary<string, ProcessCallback> ();

		public delegate bool ProcessCallback(HttpListenerRequest request, object parameter);

		public abstract string Name { get; }
		public abstract string[] AdvertisedBranches { get; }

		public abstract void Initialize(Settings modSettings);
		public abstract bool Process (HttpListenerRequest request, Parameters parameters);

		/// <summary>
		/// Gets a value indicating whether this instance is deaf for incoming requests
		/// </summary>
		/// <value><c>true</c> if this instance is deaf; otherwise, <c>false</c>.</value>
		public virtual bool IsDeaf { get { return false; } }

		public void RegisterBranch(string pin, ProcessCallback callback)
		{
			if (branches.ContainsKey (pin))
				branches.Remove (pin);

			branches.Add (pin, callback);
		}

		public bool Branch(string branch, HttpListenerRequest request, Parameters parameters)
		{
			if (branches.ContainsKey(branch))			
				return branches [branch] (request, parameters);

			return false;
		}
	}
}

