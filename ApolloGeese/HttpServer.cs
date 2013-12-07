using System;
using System.Web;
using System.Net;
using System.Collections.Generic;
using ApolloGeese.Services;

namespace ApolloGeese
{
	/// <summary>
	/// Http server.
	/// </summary>
	class HttpServer
	{
		private List<ServiceProvider> services = new List<ServiceProvider>();
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
				Secretary.Report(1, "Listening on:", prefix);
				listener.Prefixes.Add (prefix);
			}

			listener.Start();
			Secretary.Report(0, "Started Listening!");

			listener.BeginGetContext(RequestMade, listener);
			Secretary.Report(2, "Getting first context...");
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
		public ServiceProvider AddService (ServiceProvider serviceProvider, bool highPriority = false)
		{
			Secretary.Report(1, "Adding" , serviceProvider.Name, "on", (highPriority ? "high priority" : "low priority"));

			if (highPriority) services.Insert(0, serviceProvider);
			else services.Add(serviceProvider);

			return serviceProvider;
		}

		/// <summary>
		/// Request is made.
		/// </summary>
		/// <param name='ar'>
		/// Ar.
		/// </param>
		void RequestMade (IAsyncResult ar)
		{
			Secretary.Report (2, "Context Gotten");

			HttpListener contextListener = (HttpListener)ar.AsyncState;
			HttpListenerContext context = contextListener.EndGetContext (ar);

			int i = 0;
			
			Secretary.Report (2, "Matching for service...");

			// Scan for matching service indexing with 'i'
			for (; (i < services.Count) &&
			    (!services[i].Detect(context.Request));
			     i++)
				;

			// If no service awarded anything, we 404
			if (i == services.Count) {
				context.Response.StatusCode = 404;
			}
			else {
				ServiceProvider provider = services[i];
				provider.Service.Acquire(
					provider.Parse(context.Request), 
					context.Response);
			}

			context.Response.Close ();

			Secretary.Report(2, "Request Finalized!");

			listener.BeginGetContext(RequestMade, listener);
		}
	}
}

