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
				return string.Format("{0}:{1}", hostname, port);
			}
		}

		protected string hostname; protected int port;
		protected Service uriBranch, responseProcessor, postbuilder;
        private bool hasPostBuilder;
        private string sessionid;
        private bool useSesionid = false;
        private static Map<CookieCollection> sessionKeeper = new Map<CookieCollection>();

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "uri") uriBranch = e.NewValue;
			if (e.Name == "response") responseProcessor = e.NewValue;
            if (e.Name == "postbuilder")
            {
                postbuilder = e.NewValue;
                hasPostBuilder = !((postbuilder ?? Stub) is StubService);
            }
		}

		protected override void Initialize (Settings modSettings)
		{
			hostname = (string)modSettings ["hostname"];
			port = (int)modSettings["port"];
            if (modSettings.Has("sessionid"))
            {
                sessionid = modSettings.GetString("sessionid");
                useSesionid = true;
            }
		}

		/// <summary>
		/// Reads request into web request body and returns response stream.
		/// </summary>
		/// <returns>The response.</returns>
		/// <param name="request">Request.</param>
		protected virtual Stream GetResponse(Stream request, IInteraction parameters)
		{
			using (StreamReader requestReader = new StreamReader(request)) {
                HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(requestReader.ReadToEnd());
                
                string session = null;

                if (hasPostBuilder)
                {
                    webRequest.Method = "POST";
                }

                if (useSesionid)
                {
                    if (parameters.TryGetFallbackString(sessionid, out session)) 
                    {
                        if (sessionKeeper.Has(session))
                        {
                            webRequest.CookieContainer = new CookieContainer();
                            CookieCollection cc = (CookieCollection)sessionKeeper[session];

                            foreach (Cookie cookie in cc)
                            {
                                webRequest.CookieContainer.Add(cookie);
                            }
                        }                            
                    }
                    else
                        Secretary.Report(5, "No session info available at ", sessionid);                    
                }

                if (hasPostBuilder)
                {
                    QuickOutgoingInteraction postBody = 
                        new QuickOutgoingInteraction(webRequest.GetRequestStream(), parameters);

                    postbuilder.TryProcess(postBody);
                }

                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

                if (session != null)
                {
                    if (!sessionKeeper.Has(session))
                    {
                        sessionKeeper[session] = new CookieCollection();
                    }
                    CookieCollection cc = sessionKeeper[session];                     

                    for (int i = 0; i < response.Headers.Count; i++)
                    {                        
                        string key;
                        string[] values;

                        key = response.Headers.GetKey(i);
                        if (key.ToLower() == "set-cookie")
                        {
                            values = response.Headers.Get(i).Split(',');

                            foreach(string value in values) {
                                Match match = Regex.Match(value, "(.+?)=(.+?);");
                                if (match.Captures.Count > 0)
                                {
                                    string ckey = match.Groups[1].Value;
                                    string cvalue = match.Groups[2].Value;

                                    cc.Add(new Cookie(ckey, cvalue, "/", hostname));
                                }
                            }
                        }                        
                    }
                }

                return response.GetResponseStream();
			}
		}

		/// <summary>
		/// Requests for response using given Interaction
		/// </summary>
		/// <returns>The for response.</returns>
		/// <param name="parameters">Parameters.</param>
		protected virtual Stream RequestForResponse(IInteraction parameters) 
		{
			QuickOutgoingInteraction outgoingInteraction;
			WebRequest webRequest;

			using(MemoryStream uriComposingStream = new MemoryStream ()) {
				outgoingInteraction = new QuickOutgoingInteraction (uriComposingStream, parameters);

				if (!uriBranch.TryProcess (outgoingInteraction))
					throw new Exception("URI failed to compose");

				outgoingInteraction.Done();
				uriComposingStream.Position = 0;
					
				return GetResponse(uriComposingStream, parameters);
			}
		}

		protected override bool Process (IInteraction parameters)
		{			
			Stream responseStream;
			QuickIncomingInteraction incomingInteraction;

			responseStream = RequestForResponse (parameters);
			incomingInteraction = new QuickIncomingInteraction (responseStream, parameters);

			return responseProcessor.TryProcess (incomingInteraction);
		}
	}
}

