using System;
using System.Web;
using System.Net;
using System.Collections.Generic;
using BorrehSoft.ApolloGeese.Duckling;
using L = BorrehSoft.Utensils.Log.Secretary;
using BorrehSoft.Utensils.Collections.Settings;
using System.Diagnostics;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.Extensions.BasicWeblings.Server
{
	/// <summary>
	/// Http server.
	/// </summary>
	public class HttpService : Service
	{
		private HttpListener listener = new HttpListener();
		private Service httpBranch;
		private bool MeasurePerformance = true;

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

			Branches["http"] = Stub;
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "http")
				httpBranch = e.NewValue;
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
				context.Request, context.Response);

			if (!Process (parameters)) context.Response.StatusCode = 500;

			parameters.RequestBody.Close ();
			parameters.ResponseBody.Close ();
		}

		protected override bool Process (IInteraction parameters)
		{
			return httpBranch.TryProcess(parameters);
		}
	}
}

