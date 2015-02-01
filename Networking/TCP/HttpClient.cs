using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Net.Sockets;
using System.Net;
using System.IO;

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

		/// <summary>
		/// Reads request into web request body and returns response stream.
		/// </summary>
		/// <returns>The response.</returns>
		/// <param name="request">Request.</param>
		protected virtual Stream GetResponse(Stream request)
		{
			using (StreamReader requestReader = new StreamReader(request)) {
				return WebRequest.Create (requestReader.ReadToEnd()).GetResponse ().GetResponseStream ();
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
					
				return GetResponse(uriComposingStream);
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

