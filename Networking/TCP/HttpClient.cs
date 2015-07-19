using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Net.Sockets;
using System.Net;
using System.IO;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Log;
using System.Text.RegularExpressions;

namespace BorrehSoft.ApolloGeese.Extensions.Networking.TCP
{
	/// <summary>
	/// Http client, sends IOutgoing as request and produces IIncoming
	/// </summary>
	public class HttpClient : Service
	{
		public override string Description {
			get {
				return string.Format ("{0}:{1}", hostname, port);
			}
		}

		protected string hostname;
		protected int port;
		protected Service uriBranch, responseProcessor, postbuilder;
		private bool hasPostBuilder;
		private string sessionid;
		private bool useSesionid = false;
		private static Map<CookieContainer> sessionKeeper = new Map<CookieContainer> ();

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "uri")
				uriBranch = e.NewValue;
			if (e.Name == "response")
				responseProcessor = e.NewValue;
			if (e.Name == "postbuilder") {
				postbuilder = e.NewValue;
				hasPostBuilder = !((postbuilder ?? Stub) is StubService);
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			if (modSettings.Has ("default")) {
				string[] hostnamePortCombo = modSettings.GetString ("default").Split (':');

				hostname = hostnamePortCombo [0];
				if (hostnamePortCombo.Length > 1) {
					port = int.Parse (hostnamePortCombo [1]);
				} else {
					port = 80;
				}
			} else {
				hostname = (string)modSettings ["hostname"];
				port = (int)modSettings ["port"];
			}

			if (modSettings.Has ("sessionid")) {
				sessionid = modSettings.GetString ("sessionid");
				useSesionid = true;
			}
		}

		/// <summary>
		/// Preserves the cookies.
		/// </summary>
		/// <param name="webRequest">Web request.</param>
		/// <param name="id">Identifier.</param>
		void PreserveCookies (HttpWebRequest webRequest, string id)
		{
			CookieContainer container;

			if (sessionKeeper.Has (id)) {
				container = sessionKeeper [id];
			} else {
				container = sessionKeeper [id] = new CookieContainer ();
			}

			webRequest.CookieContainer = container;
		}

		/// <summary>
		/// Reads request into web request body and returns response stream.
		/// </summary>
		/// <returns>The response.</returns>
		/// <param name="request">Request.</param>
		protected virtual Stream GetResponse (Stream request, IInteraction parameters)
		{
			using (StreamReader requestReader = new StreamReader(request)) {
				HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create (requestReader.ReadToEnd ());
				QuickOutgoingInteraction postBody;

				// Preserve CookieContainer here!

				if (useSesionid) {
					string keeperid = "";
					if (parameters.TryGetString (sessionid, out keeperid)) {
						PreserveCookies (webRequest, keeperid);
					} else {
						throw new Exception (string.Format("No {0} found in context", sessionid));
					}
				}

				if (hasPostBuilder) {
					webRequest.Method = "POST";
                
					postBody = new QuickOutgoingInteraction (webRequest.GetRequestStream (), parameters);
                    
					postbuilder.TryProcess (postBody);
				}

				HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse ();
                
				return response.GetResponseStream ();
			}
		}

		/// <summary>
		/// Requests for response using given Interaction
		/// </summary>
		/// <returns>The for response.</returns>
		/// <param name="parameters">Parameters.</param>
		protected virtual Stream RequestForResponse (IInteraction parameters)
		{
			QuickOutgoingInteraction outgoingInteraction;

			using (MemoryStream uriComposingStream = new MemoryStream ()) {
				outgoingInteraction = new QuickOutgoingInteraction (uriComposingStream, parameters);

				if (!uriBranch.TryProcess (outgoingInteraction))
					throw new Exception ("URI failed to compose");

				outgoingInteraction.Done ();
				uriComposingStream.Position = 0;
					
				return GetResponse (uriComposingStream, parameters);
			}
		}

		protected override bool Process (IInteraction parameters)
		{			
			Stream responseStream;
			QuickIncomingInteraction incomingInteraction;

			responseStream = RequestForResponse (parameters);
			incomingInteraction = new QuickIncomingInteraction (responseStream, parameters, "http-response-body");

			return responseProcessor.TryProcess (incomingInteraction);
		}
	}
}
