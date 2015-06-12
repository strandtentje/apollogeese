using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket
{
	public class ReachOut : Service
	{
		public string RemoteIp { get; set; }

		public int RemotePort { get; set; }

		public override string Description {
			get {
				return "Connects to a ReachIn";
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			throw new Exception ("Return branches, we don't do yet.");
		}

		protected override void Initialize (Settings modSettings)
		{
			RemoteIp = modSettings.GetString ("host");
			RemotePort = modSettings.GetInt ("port", 43373);
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
			TcpClient connector = new TcpClient (RemoteIp, RemotePort);

			Pipe informationExchange = new Pipe (connector.Client);

			informationExchange.BeginWait (GetInformationByName, parameters);

			connector.Close ();
		}
	}
}

