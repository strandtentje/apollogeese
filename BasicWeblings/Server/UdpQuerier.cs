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

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class UdpQuerier : Service
	{
		class UdpQueryResult
		{
			public string HostIP { get; private set; }
			public string ResultString { get; private set; }
			public string Combined { get; private set; }
			public DateTime CreationTime { get; private set; }

			public UdpQueryResult (string hostIP, string resultString)
			{
				this.HostIP = hostIP;
				this.ResultString = resultString;
				this.Combined = string.Format("{0}{1}", hostIP, resultString);
			}

			public void Update ()
			{
				this.CreationTime = DateTime.Now;
			}

			public bool IsExpired (TimeSpan maxAge)
			{
				return (DateTime.Now - CreationTime) > maxAge;
			}
		}

		public override string Description {
			get {
				return "Querier and iterator for UDP packets";
			}
		}

		Dictionary<string, UdpQueryResult> results = new Dictionary<string, UdpQueryResult>();
		UdpClient client = new UdpClient();
		IPAddress IP;
		Service iteratorBranch;
		Thread gatherThread;
		Timer queryThread;
		string queryText;
		byte[] queryTextBytes;
		int Port, queryInterval;

		protected override void Initialize (Settings modSettings)
		{
			queryInterval = int.Parse(modSettings.GetString("minutesbetweenqueries", 1)) * 60000;
			IP = IPAddress.Parse(modSettings.GetString("ip", "255.255.255.255"));
			Port = int.Parse(modSettings.GetString("port", "15325"));

			queryText = modSettings.GetString("querytext", "test");
			queryTextBytes = Encoding.ASCII.GetBytes(queryText);

			queryThread = new Timer(queryMethod, null, 0, queryInterval);

			BeginGathering();
		}

		private void queryMethod()
		{
			IPEndPoint endpoint = new IPEndPoint(IP, Port);

			client.Send(queryTextBytes, queryTextBytes.Length, endpoint);
		}

		private void BeginGathering()
		{
			IPEndPoint endpoint = new IPEndPoint(IP, Port);

			client.BeginReceive(Gather, endpoint);
		}

		private void Gather (IAsyncResult asyncResult)
		{
			IPEndPoint endpoint = asyncResult.AsyncState as IPEndPoint;
			byte[] result = client.EndReceive (asyncResult, ref endpoint);
			string resultString = Encoding.ASCII.GetString (result);

			UdpQueryResult resultRow = new UdpQueryResult (endpoint.Address.ToString (), resultString);

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
				iteratorBranch.TryProcess(new QuickInteraction(parameters, result));

			return true;
		}

	}
}

