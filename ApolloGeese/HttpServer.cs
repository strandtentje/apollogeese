using System;
using System.Web;
using System.Net;
using System.Collections.Generic;
using BorrehSoft.ApolloGeese.Duckling;
using L = BorrehSoft.Utensils.Log.Secretary;

namespace BorrehSoft.ApolloGeese
{
	/// <summary>
	/// Http server.
	/// </summary>
	class HttpServer
	{
		private List<Service> services = new List<Service>();
		private HttpListener listener = new HttpListener();

		/// <summary>
		/// Initializes a new instance of the <see cref="ApolloGeese.HttpServer"/> class.
		/// </summary>
		/// <param name='prefixes'>
		/// Prefixes to listen on.
		/// </param>
		public HttpServer (params string[] prefixes)
		{
			foreach (string prefix in prefixes) {
				L.Report(5, "Listening on:", prefix);
				listener.Prefixes.Add (prefix);
			}

			listener.Start();
			L.Report(4, "Started Listening!");

			listener.BeginGetContext(RequestMade, listener);
			L.Report(5, "Getting first context...");
		}

		/// <summary>
		/// Adds a service.
		/// </summary>
		/// <returns>
		/// The added service.
		/// </returns>
		/// <param name='serviceProvider'>
		/// Service provider to add
		/// </param>
		/// <param name='highPriority'>
		/// Set to true if this service is to be checked for first
		/// when handling a request.
		/// </param>
		public Service AddService (Service service, bool highPriority = false)
		{
			L.Report(5, "Adding" , service.Name, "on", (highPriority ? "high priority" : "low priority"));

			if (highPriority) services.Insert(0, service);
			else services.Add(service);

			return service;
		}

		/// <summary>
		/// Request is made.
		/// </summary>
		/// <param name='ar'>
		/// Ar.
		/// </param>
		void RequestMade (IAsyncResult ar)
		{
			listener.BeginGetContext(RequestMade, listener);

			L.Report (5, "Context Gotten");

			HttpListener contextListener = (HttpListener)ar.AsyncState;
			HttpListenerContext context = contextListener.EndGetContext (ar);

			int i = 0;
			
			L.Report (5, "Matching for service...");

			// Scan for matching service indexing with 'i'
			for (; (i < services.Count) &&
			    (!services[i].Process(context.Request, null));
			     i++)
				;

			// If no service awarded anything, we 404
			if (i == services.Count) {
				context.Response.StatusCode = 500;
			}

			context.Response.Close ();

			L.Report(5, "Request Finalized!");
		}
	}
}

