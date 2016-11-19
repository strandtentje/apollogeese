using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.ApolloGeese.Extensions.Networking;
using BorrehSoft.Utilities.Collections;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket.Piping;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket
{
	public class ReachOut : IPService
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

		BlockingPool<TcpClient> pool;

		bool CheckTcpClient (TcpClient arg)
		{
			return arg.Connected;
		}

		TcpClient GetTcpClient ()
		{
			return new TcpClient (Ip, Port);
		}

		void ResetTcpClient (TcpClient obj)
		{
			obj.Close ();
		}

		protected override void Initialize (Settings modSettings)
		{
			base.Initialize (modSettings);

			pool = new BlockingPool<TcpClient> (
				modSettings.GetInt ("poolsize", 4), 
				GetTcpClient, ResetTcpClient, CheckTcpClient);
		}

		public override void Dispose ()
		{
			pool.Dispose ();
			base.Dispose ();
		}

		protected override bool Process (IInteraction parameters)
		{
			pool.Fetch(delegate(TcpClient connector) {
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
			});

			return true;
		}
	}
}

