using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.Networking.TCP
{
	public class TcpServer// : SocketService
	{
		public override string Description {
			get {
				return ""; // return string.Format ("TCP Listening on {0}:{1}", this.IP, this.Port);
			}
		}

		protected override void Initialize (Settings settings)
		{
			
		}
	}
}

