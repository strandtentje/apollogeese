using System;
using System.Web;
using System.Net;
using System.Collections.Generic;
using BorrehSoft.ApolloGeese.Duckling;
using L = BorrehSoft.Utensils.Log.Secretary;
using BorrehSoft.Utensils.Settings;
using System.Diagnostics;

namespace BorrehSoft.Extensions.HttpService
{
	/// <summary>
	/// Http server.
	/// </summary>
	public class HttpService : Service
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

		public override string Description {
			get {
				return "HttpServer";
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			listener.Stop ();

			listener.Prefixes.Clear ();

			foreach(object prefix in (List<object>)modSettings["prefixes"])			
				listener.Prefixes.Add((string)prefix);

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

			Stopwatch requestSw = new Stopwatch ();

			Interaction parameters = new Interaction (context.Request);

			requestSw.Start ();
			bool succesful = Process (parameters);
			requestSw.Stop ();

			if (succesful) {
				parameters.Ready (context.Response);
			}
			else {
				context.Response.StatusCode = 500;
			}

			context.Response.OutputStream.Close ();
			context.Response.Close ();

			L.Report(5, "Request Finalized in", requestSw.ElapsedMilliseconds.ToString(), "milliseconds");
		}

		protected override bool Process (Interaction parameters)
		{
			return RunBranch ("http", parameters);
		}
	}
}

