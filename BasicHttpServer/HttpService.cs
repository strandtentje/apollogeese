using System;
using System.Web;
using System.Net;
using System.Collections.Generic;
using BorrehSoft.ApolloGeese.CoreTypes;
using L = BorrehSoft.Utensils.Log.Secretary;
using BorrehSoft.Utensils.Collections.Settings;
using System.Diagnostics;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.ApolloGeese.Extensions.BasicHttpServer
{
	/// <summary>
	/// Http server.
	/// </summary>
	public class HttpService : Service
	{
		private HttpListener listener = new HttpListener();
		private Service httpBranch;
		private List<string> prefixStrings = new List<string>();

		public override string Description {
			get {
				return "HttpServer";
			}
		}

		[Instruction("Produces performance measurements into log file when set to true.", false)]
		public bool MeasurePerformance { get; set; }

		[Instruction("URL prefixes that this server will handle", new string[] {})]
		public List<string> Prefixes { 
			get {
				return this.prefixStrings;
			}
			set {
				this.prefixStrings = value;

				foreach (string prefix in this.prefixStrings) {
					if (!listener.Prefixes.Contains (prefix)) {
						listener.Prefixes.Add ((string)prefix);
					}
				}

				this.prefixStrings.RemoveAll (delegate(string foundPrefix) {
					return this.prefixStrings.Contains(foundPrefix) == false;
				});
			}
		}


		protected override void Initialize (Settings modSettings)
		{
			MeasurePerformance = modSettings.GetBool("measureperformance", false);
			Prefixes = modSettings.GetStringList ("prefixes");

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

			HttpListener contextListener = (HttpListener)ar.AsyncState;
			HttpListenerContext context = contextListener.EndGetContext (ar);

			L.Report (5, "Request opened by", context.Request.UserHostAddress);

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
            
			if (Process (parameters)) {
				if (!parameters.IsStatuscodeSet) {
					parameters.SetStatuscode (200);
				}
			} else {
				context.Response.StatusCode = 500;
			}

            try
            {
			    parameters.IncomingBody.Close ();
            }
            catch (Exception ex)
            {
                Secretary.Report(5, "Failure to close incoming stream", ex.Message);
            }

            try
            {
                parameters.OutgoingBody.Close();
            }
            catch (Exception ex)
            {
                Secretary.Report(5, "Failure to close outgoing stream", ex.Message);
            }
		}

		protected override bool Process (IInteraction parameters)
		{
			return httpBranch.TryProcess(parameters);
		}

		public override void Dispose ()
		{
			this.listener.Stop ();
			this.listener.Close ();
		}
	}
}

