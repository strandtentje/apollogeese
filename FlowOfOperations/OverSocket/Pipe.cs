using System;
using System.Net.Sockets;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket
{
	public class Pipe
	{
		public delegate object InformationSourceDelegate (string name, object state);

		public Socket Socket { get; private set; }

		public Pipe (Socket socket)
		{
			this.Socket = socket;
		}

		void SendSymbol (Symbol symbol)
		{
			byte[] buffer = new byte[1];
			buffer [0] = (byte)symbol;

			Socket.Send (buffer);
		}

		public void BeginWait (InformationSourceDelegate getInformationByName, IInteraction parameters)
		{
			SendSymbol (Symbol.Hi);
			AwaitSymbol 
		}


	}
}

