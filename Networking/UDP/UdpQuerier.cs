using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Collections.Maps;
using System.Collections.Generic;
using System.Collections;

namespace BorrehSoft.ApolloGeese.Extensions.Networking.UDP
{
	/// <summary>
	/// UDP querier.
	/// </summary>
	public class UdpQuerier : Service
	{
		public override string Description {
			get {
				return "Querier and iterator for UDP packets";
			}
		}

		Dictionary<string, UdpQueryResult> results = new Dictionary<string, UdpQueryResult>();
        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        IPAddress IP; EndPoint sendpoint; EndPoint receivepoint;
		Service iteratorBranch;
		Thread gatherThread;
		Timer queryThread;
		string queryText;
		byte[] queryTextBytes;
		int Port, queryInterval;

		protected override void Initialize (Settings modSettings)
		{
			queryInterval = int.Parse(modSettings.GetString("minutesbetweenqueries", "1")) * 60000;
			IP = IPAddress.Parse(modSettings.GetString("ip", "255.255.255.255"));
			Port = int.Parse(modSettings.GetString("port", "15325"));
            sendpoint = new IPEndPoint(IP, Port);
            receivepoint = new IPEndPoint(IPAddress.Any, Port);
                        
			queryText = modSettings.GetString("querytext", "test");
			queryTextBytes = Encoding.ASCII.GetBytes(queryText);

			queryThread = new Timer(queryMethod, null, 0, queryInterval);

			BeginGathering();
		}

		private void queryMethod(object o)
		{
            client.SendTo(queryTextBytes, queryTextBytes.Length, SocketFlags.Broadcast, sendpoint);
		}

		private void BeginGathering()
		{
            byte[] buffer = new byte[1024];
            client.BeginReceiveFrom(buffer, 0, 1024, SocketFlags.None, ref receivepoint, Gather, client);
		}

		private void Gather (IAsyncResult asyncResult)
		{            
			byte[] result = client.EndReceiveFrom (asyncResult, ref receivepoint);
			string resultString = Encoding.ASCII.GetString (result);

			UdpQueryResult resultRow = new UdpQueryResult (sendpoint.Address.ToString (), resultString);

			if (!results.ContainsKey (resultRow.Combined)) results.Add(resultRow.Combined, resultRow);

			results[resultRow.Combined].Update();

			BeginGathering();
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "iterator") {
				iteratorBranch = e.NewValue ?? Service.Stub;
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			foreach (UdpQueryResult result in results.Values) 
				iteratorBranch.TryProcess(result.Clone(parameters));

			return true;
		}

	}
}

