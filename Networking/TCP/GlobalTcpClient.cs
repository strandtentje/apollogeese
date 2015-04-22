using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace BorrehSoft.ApolloGeese.Extensions.Networking.TCP
{
	/// <summary>
	/// Stateful TCP client that stays available for reading even until after
	/// the initiating branch is done with it.
	/// </summary>
	public class GlobalTcpClient : HttpClient
	{
		private TcpClient singleClient = new TcpClient();

		public override string Description {
			get {
				return string.Format("Single TCP client to {0}:{1}", hostname, port);
			}
		}

		/// <summary>
		/// Gets response stream for a new request stream
		/// </summary>
		/// <returns>The response.</returns>
		/// <param name="body">Body.</param>
		protected override Stream GetResponse (Stream body, IInteraction parameters)
		{
			singleClient = new TcpClient (hostname, port);
			body.CopyTo (singleClient.GetStream ());

			return singleClient.GetStream ();
		}

		protected override Stream RequestForResponse (IInteraction parameters)
		{
			if (!singleClient.Connected)
				return base.RequestForResponse (parameters);
			else
				return singleClient.GetStream ();
		}
	}
}

