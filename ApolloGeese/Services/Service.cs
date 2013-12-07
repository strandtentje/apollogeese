using System;
using System.Net;

namespace ApolloGeese.Services
{
	public abstract class Service
	{
		public abstract string Name { get; }

		public void Acquire (ServiceParams serviceParams, HttpListenerResponse response)
		{

		}
	}

}

