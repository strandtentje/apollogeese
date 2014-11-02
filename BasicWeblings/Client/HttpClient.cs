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
				return "Does outgoing HTTP-request";
			}
		}

		string hostname; int port;
		Service uriBranch, responseProcessor;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "uri") uriBranch = e.NewValue;
			if (e.Name == "response") responseProcessor = e.NewValue;
		}

		protected override void Initialize (Settings modSettings)
		{
			hostname = modSettings["hostname"] as String;
			port = modSettings.GetInt("port", 80);
		}

		protected override bool Process (IInteraction parameters)
		{
			bool success;
			HttpOutgoingInteraction outgoingInteraction;
			HttpResponseInteraction incomingInteraction;
			WebRequest webRequest;
			Stream responseStream;

			using(MemoryStream uriComposingStream = new MemoryStream ()) {
				outgoingInteraction = new HttpOutgoingInteraction (uriComposingStream, parameters);

				if (!uriBranch.TryProcess (outgoingInteraction))
					throw new Exception("URI failed to compose");

				outgoingInteraction.Done();
				uriComposingStream.Position = 0;

				using(StreamReader uriReader = new StreamReader(uriComposingStream)) {				
					webRequest = WebRequest.Create(uriReader.ReadToEnd());
				}
			}

			responseStream  = webRequest.GetResponse().GetResponseStream();
			incomingInteraction = new HttpResponseInteraction (responseStream, parameters);

			return responseProcessor.TryProcess (incomingInteraction);
		}
	}
}

