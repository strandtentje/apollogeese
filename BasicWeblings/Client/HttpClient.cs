using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Net.Sockets;
using System.Net;

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
		Service outgoingBodyBranch, responseProcessor;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "body") outgoingBodyBranch = e.NewValue;
			if (e.Name == "response") responseProcessor = e.NewValue;
		}

		protected override void Initialize (Settings modSettings)
		{
			hostname = modSettings["hostname"] as String;
			port = modSettings.GetInt("port", 80);
		}

		protected override bool Process (IInteraction parameters)
		{
			bool outgoingSuccess, incomingSuccess;

			using (TcpClient newClient = new TcpClient()) {
				newClient.Connect (hostname, port);

				outgoingSuccess = outgoingBodyBranch.TryProcess (new HttpOutgoingInteraction (newClient.GetStream ())); 
				incomingSuccess = responseProcessor.TryProcess (new HttpResponseInteraction (newClient.GetStream ()));
			}	

			return outgoingSuccess && incomingSuccess;
		}
	}
}

