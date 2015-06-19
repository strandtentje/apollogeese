using System;
using System.Net.Sockets;
using System.Net;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket.Networking
{
	/// <summary>
	/// New client event handler.
	/// </summary>
	public delegate void NewClientEventHandler (object sender, NewClientEventArgs e);

	/// <summary>
	/// New client event arguments.
	/// </summary>
	public class NewClientEventArgs 
	{
		public TcpClient Client { get; set; }

		public NewClientEventArgs (TcpClient newClient)
		{
			this.Client = newClient;
		}
	}
}

