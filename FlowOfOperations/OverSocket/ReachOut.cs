using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket.Piping;
using BorrehSoft.ApolloGeese.Extensions.Networking;

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

		bool Closer(Pipe pipe, IInteraction parameters) {
			return false;
		}

		bool Composer (Pipe pipe, IInteraction parameters) {
			string id = pipe.ReceiveString ();

			object candidate;

			if (parameters.TryGetFallback (id, out candidate)) {
				pipe.SendString (candidate.ToString ());
			}

			return true;
		}

		bool Stringer (Pipe pipe, IInteraction parameters) {
			string id = pipe.ReceiveString ();

			string candidate;

			if (parameters.TryGetFallbackString (id, out candidate)) {
				pipe.SendString (candidate);
			}

			return true;
		}

		bool Typer (Pipe pipe, IInteraction parameters)
		{
			string id = pipe.ReceiveString ();

			object candidate;

			if (parameters.TryGetFallback (id, out candidate)) {
				pipe.SendString (candidate.GetType ().ToString ());
			} else {
				pipe.SendString (ReachInteraction.NullTypeName);
			}

			return true;
		}

		delegate bool CommandFulfiller(Pipe pipe, IInteraction parameters);

		CommandFulfiller GetPipeCommandCallback(int command) {
			if (command == Command.Close)
				return Closer;
			if (command == Command.Compose)
				return Composer;
			if (command == Command.String)
				return Stringer;
			if (command == Command.Type)
				return Typer;

			return delegate(Pipe pipe, IInteraction parameters) {
				return true;
			};
		}

		protected override bool Process (IInteraction parameters)
		{
			TcpClient connector = new TcpClient (Ip, Port);

			Pipe informationExchange = new Pipe (connector.Client);

			informationExchange.Handshake ();

			while (
				GetPipeCommandCallback(
					informationExchange.ReceiveCommand ())(
						informationExchange, parameters));
							// dikkertje dap
								// zat op de trap
									// 's morgens vroeg om kwart over zeven
										// zijn collega had spaghetticode geschreven




			connector.Close ();

			return true;

		}
	}
}

