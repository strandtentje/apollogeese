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
	public class ReachIn : SocketService
	{
		public override string Description {
			get {
				return "Awaits connection from a reachout";
			}
		}

		ListenerWithEvents listener;

		Service begin;

		protected override void Initialize (Settings modSettings)
		{
			base.Initialize (modSettings);
			listener = new ListenerWithEvents (Ip, Port);
			listener.NewClient += HandleNewClient;
		}

		void HandleNewClient (object sender, NewClientEventArgs e)
		{
			Pipe pipe = new Pipe (e.Client.Client);

			pipe.Handshake ();

			begin.TryProcess (new ReachInteraction (pipe));

			pipe.SendCommand (Command.Close);

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
