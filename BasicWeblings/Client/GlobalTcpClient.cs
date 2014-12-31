using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace BorrehSoft.Extensions.BasicWeblings.Client
{
	public class GlobalTcpClient : HttpClient
	{
		private TcpClient singleClient = new TcpClient();

		public override string Description {
			get {
				return string.Format("Single TCP client to {0}:{1}", hostname, port);
			}
		}

		protected override Stream GetResponse (Stream body)
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

