using System;
using System.Net.Sockets;
using System.Net;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket.Networking
{
	/// <summary>
	/// Listener with events.
	/// </summary>
	public class ListenerWithEvents : TcpListener
	{
		public event NewClientEventHandler NewClient;

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket.Networking.ListenerWithEvents"/> class
		/// and starts listening
		/// </summary>
		/// <param name="ip">Ip.</param>
		/// <param name="port">Port.</param>
		public ListenerWithEvents (string ip, int port) : base(IPAddress.Parse(ip), port)
		{
			this.Start ();

			reListen ();
		}

		/// <summary>
		/// Start listening
		/// </summary>
		void reListen() {
			this.BeginAcceptTcpClient (newClientFound, new object());
		}

		/// <summary>
		/// New client found.
		/// </summary>
		/// <param name="ar">Ar.</param>
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

