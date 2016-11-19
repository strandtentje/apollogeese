using System;
using System.Net;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;
using System.Net.Sockets;

namespace BorrehSoft.ApolloGeese.Extensions.Networking
{
	/// <summary>
	/// I hope you weren't expecting anything exciting here
	/// </summary>
	public abstract class IPService : SingleBranchService
	{
		[Instruction("IP of socket")]
		public string Ip { get; set; }

		[Instruction("Port of socket", 43373)]
		public int Port { get; set; }

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
