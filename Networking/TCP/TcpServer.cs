using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using System.Net.Sockets;
using System.Net;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.ApolloGeese.Extensions.Networking.TCP
{
	public class TcpServer : IPService
	{
		public override string Description {
			get {
				return string.Format ("TCP Listening on {0}:{1}", this.IP, this.Port);
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			base.Initialize (modSettings);

			TcpListener listener = new TcpListener (IPAddress.Parse(this.Ip), this.Port);

			listener.BeginAcceptTcpClient (IncomingConnection, listener);
		}

		private Service Connection;
		private long connectionCallsignTicker = 0;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "connection")
				this.Connection = e.NewValue;
		}

		private void IncomingConnection(IAsyncResult result) {
			TcpListener listener = result.AsyncState as TcpListener;
			listener.BeginAcceptTcpClient (IncomingConnection, listener);

			using (TcpClient client = listener.EndAcceptTcpClient (result)) {

				long myTicker = connectionCallsignTicker++;

				Secretary.Report (8, 
					"Tcp connection", myTicker,
					"made from", client.Client.RemoteEndPoint.ToString ());

				if (this.Connection.TryProcess (new TcpInteraction (client.Client, myTicker))) {
					Secretary.Report (8, "Tcp connection", myTicker, "deal with successfully");
				} else {
					Secretary.Report (5, "Tcp connection", myTicker, "failed to be dealt with successfully");
				}
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			
		}
	}
}

