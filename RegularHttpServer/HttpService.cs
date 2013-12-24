using System;
using System.Web;
using System.Net;
using System.Collections.Generic;
using BorrehSoft.ApolloGeese.Duckling;
using L = BorrehSoft.Utensils.Log.Secretary;
using BorrehSoft.Utensils.Settings;

namespace BorrehSoft.Extensions.HttpService
{
	/// <summary>
	/// Http server.
	/// </summary>
	class HttpService : Service
	{
		private HttpListener listener = new HttpListener();

		/// <summary>
		/// Gets a value indicating whether this instance is deaf for passed-on requests.
		/// This is true for HttpService for it is the component with the httplistener.
		/// </summary>
		/// <value>true</value>
		/// <c>false</c>
		public override bool IsDeaf {
			get {
				return true;
			}
		}

		public override string[] AdvertisedBranches {
			get {
			return new string[] { "http" };
			}
		}

		public override string Name {
			get {
				return "HttpServer";
			}
		}

		public override void Initialize (Settings modSettings)
		{
			foreach(string prefix in (string[])modSettings["prefixes"])			
				listener.Prefixes.Add(prefix);

			listener.Start ();

			listener.BeginGetContext (RequestMade, listener);
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

			if (!Process (context, null))
				context.Response.StatusCode = 500;

			context.Response.Close ();

			L.Report(5, "Request Finalized!");
		}

		public override bool Process (HttpListenerContext context, Parameters parameters)
		{
			return RunBranch ("http", context, parameters);
		}
	}
}

