using System;
using System.Web;
using System.Net;
using System.Collections.Generic;
using BorrehSoft.ApolloGeese.Duckling;
using L = BorrehSoft.BorrehSoft.Utensils.Collections.Log.Secretary;
using BorrehSoft.BorrehSoft.Utensils.Collections.Settings;
using System.Diagnostics;

namespace BorrehSoft.Extensions.BasicWeblings.Server
{
	/// <summary>
	/// Http server.
	/// </summary>
	public class HttpService : Service
	{
		private HttpListener listener = new HttpListener();
		private bool MeasurePerformance = true;

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
			if (modSettings ["MeasurePerformance"] is string) {
				if (bool.TryParse ((string)modSettings ["MeasurePerformance"],
				                   out MeasurePerformance)) {
					MeasurePerformance = true;
				}
			}

			listener.Stop ();
			listener.Prefixes.Clear ();

			List<object> PrefixObjs = (List<object>)modSettings ["prefixes"];

			foreach(object prefix in PrefixObjs)			
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

			if (!MeasurePerformance) {
				EnterTree (context);
				return;
			}

			Stopwatch requestSw = new Stopwatch ();
			requestSw.Start ();

			EnterTree (context);

			requestSw.Stop ();
			L.Report (5, "Request Finalized in", requestSw.ElapsedMilliseconds.ToString (), "milliseconds");
		}

		/// <summary>
		/// Enters the tree, onwards!
		/// </summary>
		/// <param name="context">Context.</param>
		void EnterTree (HttpListenerContext context)
		{
			HttpInteraction parameters = new HttpInteraction (
				context.Request, context.Response) { 
				Name = "Basic Http Interaction" 
			};

			if (!Process (parameters)) context.Response.StatusCode = 500;

			parameters.RequestBody.Close ();
			parameters.ResponseBody.Close ();
		}

		protected override bool Process (IInteraction parameters)
		{
			return RunBranch ("http", parameters);
		}
	}
}

