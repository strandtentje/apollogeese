using System;
using System.Net.Sockets;
using System.Net;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	/// <summary>
	/// Listener with events.
	/// </summary>
	public class ListenerWithEvents : TcpListener
	{
		public event NewClientEventHandler NewClient;

		public ListenerWithEvents (string ip, int port) : base(IPAddress.Parse(ip), port)
		{
			reListen ();
		}

		void reListen() {
			this.BeginAcceptTcpClient (newClientFound, new object());
		}

		void newClientFound (IAsyncResult ar)
		{
			reListen ();

			TcpClient newClient = this.EndAcceptTcpClient (ar);

			if (NewClient != null) {
				NewClient (this, new NewClientEventArgs (newClient));
			}
		}
	}
}

