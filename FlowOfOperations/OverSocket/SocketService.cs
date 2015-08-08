using System;
using System.Net;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Net.Sockets;
using BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket.Networking;
using BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket.Piping;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.OverSocket
{
	/// <summary>
	/// I hope you weren't expecting anything exciting here
	/// </summary>
	public abstract class SocketService : Service
	{
		protected string Ip { get; set; }

		protected int Port { get; set; }

		protected override void Initialize (Settings modSettings)
		{
			Ip = modSettings.GetString ("host");
			Port = modSettings.GetInt ("port", 43373);
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			string[] connectionPair = defaultParameter.Split (':');

			Settings ["host"] = connectionPair [0];

			if (connectionPair.Length > 1) {
				int port;
				if (int.TryParse (connectionPair [1], out port)) {
					Settings ["port"] = port;
				} else {
					throw new ArgumentException ("port needs to be numeric int value");
				}
			}

		}
	}

}
