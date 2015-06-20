using System;
using System.Net;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Net.Sockets;
using BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket.Networking;
using BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket.Piping;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket
{
	public class ReachIn : Service
	{
		public override string Description {
			get {
				return "Awaits connection from a reachout";
			}
		}

		ListenerWithEvents listener;

		Service begin;

		string LocalIp { get; set; }

		int LocalPort { get; set; }

		protected override void Initialize (Settings modSettings)
		{
			LocalIp = modSettings.GetString ("host");
			LocalPort = modSettings.GetInt ("port", 43373);

			listener = new ListenerWithEvents (LocalIp, LocalPort);
			listener.NewClient += HandleNewClient;
		}

		void HandleNewClient (object sender, NewClientEventArgs e)
		{
			Pipe pipe = new Pipe (e.Client.Client);

			begin.TryProcess (new ReachInteraction (pipe));

			e.Client.Close ();
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "begin")
				this.begin = e.NewValue;
		}

		protected override bool Process (IInteraction parameters)
		{

			return false;	
		}
	}
}
