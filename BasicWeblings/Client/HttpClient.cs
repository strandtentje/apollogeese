using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace BorrehSoft.Extensions.BasicWeblings.Client
{
	public class HttpClient : Service
	{
		public override string Description {
			get {
				return string.Format("{0}:{1}", hostname, port);
			}
		}

		protected string hostname; protected int port;
		protected Service uriBranch, responseProcessor;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "uri") uriBranch = e.NewValue;
			if (e.Name == "response") responseProcessor = e.NewValue;
		}

		protected override void Initialize (Settings modSettings)
		{
			hostname = (string)modSettings ["hostname"];
			port = (int)modSettings["port"];
		}

		protected virtual Stream GetResponse(Stream request)
		{
			using (StreamReader requestReader = new StreamReader(request)) {
				return WebRequest.Create (requestReader.ReadToEnd()).GetResponse ().GetResponseStream ();
			}
		}

		protected virtual Stream RequestForResponse(IInteraction parameters) 
		{
			HttpOutgoingInteraction outgoingInteraction;
			WebRequest webRequest;

			using(MemoryStream uriComposingStream = new MemoryStream ()) {
				outgoingInteraction = new HttpOutgoingInteraction (uriComposingStream, parameters);

				if (!uriBranch.TryProcess (outgoingInteraction))
					throw new Exception("URI failed to compose");

				outgoingInteraction.Done();
				uriComposingStream.Position = 0;
					
				return GetResponse(uriComposingStream);
			}
		}

		protected override bool Process (IInteraction parameters)
		{			
			Stream responseStream;
			HttpResponseInteraction incomingInteraction;

			responseStream = RequestForResponse (parameters);
			incomingInteraction = new HttpResponseInteraction (responseStream, parameters);

			return responseProcessor.TryProcess (incomingInteraction);
		}
	}
}
