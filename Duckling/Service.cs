using System;
using System.Net;

namespace BorrehSoft.ApolloGeese.Duckling
{
	public abstract class Service
	{
		public abstract string Name { get; }

		public void Run (ServiceParams serviceParams, HttpListenerResponse response)
		{

		}
	}

}

