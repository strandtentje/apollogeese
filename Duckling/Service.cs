using System;
using System.Net;
using System.Collections.Generic;
using BorrehSoft.Utensils.Settings;

namespace BorrehSoft.ApolloGeese.Duckling
{
	public abstract class Service	
	{
		private List<ProcessCallback> outPipes = new List<ProcessCallback> ();

		public delegate bool ProcessCallback(HttpListenerRequest request, object parameter);

		public abstract string Name { get; }
		public abstract string[] AdvertisedPipes { get; }

		public abstract void Initialize(Settings modSettings);
		public abstract bool Process (HttpListenerRequest request, Parameters parameters);

		public void RegisterOutpipe(ProcessCallback callback)
		{
			outPipes.Add (callback);
		}

		public bool PipeOut(HttpListenerRequest request, Parameters parameters)
		{
			for (int i = 0; i < outPipes.Count; i++)
				if (outPipes[i] (request, parameters))
					return true;

			return false;
		}
	}
}

