using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket.Piping;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket
{
	public class ReachOut : SocketService
	{
		public override string Description {
			get {
				return "Connects to a ReachIn";
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			throw new Exception ("Return branches, we don't do yet.");
		}

		object GetInformationByName (string name, object state)
		{
			object output;
			IInteraction interaction = (IInteraction)state;

			interaction.TryGetFallback (name, out output);

			return output;
		}

		protected override bool Process (IInteraction parameters)
		{
			TcpClient connector = new TcpClient (Ip, Port);

			Pipe informationExchange = new Pipe (connector.Client);

			informationExchange.BeginWait (GetInformationByName, parameters);

			connector.Close ();

			return true;

		}
	}
}

