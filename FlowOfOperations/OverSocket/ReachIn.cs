using System;
using System.Net;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Net.Sockets;
using BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket.Networking;
using BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket.Piping;
using BorrehSoft.ApolloGeese.Extensions.Networking;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket
{
	public class ReachIn : IPService
	{
		public override string Description {
			get {
				return string.Format("ReachIn at {0}:{1}", this.Ip, this.Port);
			}
		}

		ListenerWithEvents listener;

		[Instruction("Love isn't always on time")]
		public bool HoldTheLine {
			get;
			private set;
		}

		protected override void Initialize (Settings modSettings)
		{
			base.Initialize (modSettings);
			listener = new ListenerWithEvents (Ip, Port);
			listener.NewClient += HandleNewClient;

			HoldTheLine = modSettings.GetBool ("holdtheline", true);
		}

		void HandleNewClient (object sender, NewClientEventArgs e)
		{	
			try {
				Pipe pipe = new Pipe (e.Client.Client);

				while(this.HoldTheLine) {
					pipe.Handshake ();
					WithBranch.TryProcess (new ReachInteraction (pipe));
					pipe.SendCommand (Command.Close);
				}

				e.Client.Close ();
			} catch (SocketException ex) {
				Secretary.Report (5, "Connection failed at", this.Description);
			} catch (Exception ex) {
				Secretary.Report (5, "Misc. exception occured at", this.Description, ":", ex.Message);
				e.Client.Close ();
			}
		}

		protected override bool Process (IInteraction parameters)
		{

			return false;	
		}
	}
}
