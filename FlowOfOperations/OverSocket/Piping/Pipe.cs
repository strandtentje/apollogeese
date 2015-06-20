using System;
using System.Net.Sockets;
using BorrehSoft.ApolloGeese.Duckling;
using System.Text;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket.Piping
{
	/// <summary>
	/// Pipe.
	/// </summary>
	public class Pipe
	{
		/// <summary>
		/// Information source delegate.
		/// </summary>
		public delegate object InformationSourceDelegate (string name, object state);

		/// <summary>
		/// Gets the socket.
		/// </summary>
		/// <value>The socket.</value>
		public Socket Socket { get; private set; }

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket.Pipe`1"/> class.
		/// </summary>
		/// <param name="socket">Socket.</param>
		public Pipe (Socket socket)
		{
			this.Socket = socket;
		}

		/// <summary>
		/// Sends a low-level symbol.
		/// </summary>
		/// <param name="symbol">Symbol.</param>
		void SendSymbol (Symbol symbol)
		{
			byte[] buffer = new byte[1];
			buffer [0] = (byte)symbol;

			Socket.Send (buffer);
		}

		/// <summary>
		/// Awaits a low level symbol.
		/// </summary>
		/// <param name="symbol">Symbol.</param>
		void AwaitSymbol(Symbol symbol) 
		{
			byte[] buffer = new byte[1];
			Socket.Receive (buffer, 1, SocketFlags.None);

			byte incoming = buffer [0];

			if (incoming != (byte)symbol) {
				string receivedSymbol = "unknown";
				if (Enum.IsDefined (typeof(Symbol), incoming)) {
					receivedSymbol = ((Symbol)incoming).ToString ();
				}

				throw new PipeException (string.Format ("Was awaiting symbol {0} but received {1} ({2}) instead.",
					symbol.ToString (), receivedSymbol, incoming.ToString ()));
			}
		}

		/// <summary>
		/// Does a handshake and blocks the thread until the interaction was
		/// satisfactory.
		/// </summary>
		/// <param name="getInformationByName">Get information by name.</param>
		/// <param name="parameters">Parameters.</param>
		public void BeginWait (InformationSourceDelegate getInformationByName, IInteraction parameters)
		{
			SendSymbol (Symbol.Hi);
			AwaitSymbol (Symbol.Hi);
		}

		/// <summary>
		/// Sends an int
		/// </summary>
		/// <returns>The int.</returns>
		/// <param name="number">Number.</param>
		public void SendInt(int number) {
			SendSymbol (Symbol.Int);
			Socket.Send (BitConverter.GetBytes (number));
		}

		/// <summary>
		/// Sends a string.
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="text">Text.</param>
		public void SendString(string text) {
			SendSymbol (Symbol.String);
			byte[] buffer = Encoding.Unicode.GetBytes (text);
			SendInt (buffer.Length);
			Socket.Send (buffer);
		}

		/// <summary>
		/// Sends the command.
		/// </summary>
		/// <param name="command">Command.</param>
		public void SendCommand(int command) {
			SendSymbol (Symbol.Command);
			SendInt (command);
		}

		/// <summary>
		/// Receives an int.
		/// </summary>
		/// <returns>The int.</returns>
		public int ReceiveInt() {
			AwaitSymbol (Symbol.Int);
			byte[] buffer = new byte[sizeof(int)];
			Socket.Receive (buffer, 4, SocketFlags.None);
			return BitConverter.ToInt32 (buffer, 0);
		}

		/// <summary>
		/// Receives a string.
		/// </summary>
		/// <returns>The string.</returns>
		public string ReceiveString() {
			AwaitSymbol (Symbol.String);
			byte[] buffer = new byte[ReceiveInt ()];
			Socket.Receive (buffer, buffer.Length, SocketFlags.None);
			return Encoding.Unicode.GetString (buffer);
		}

		/// <summary>
		/// Receives a command.
		/// </summary>
		/// <returns>The command.</returns>
		public int ReceiveCommand() {
			AwaitSymbol (Symbol.Command);
			return ReceiveInt ();
		}
	}
}

