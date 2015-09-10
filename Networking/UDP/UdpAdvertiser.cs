using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.Networking.UDP
{
	/// <summary>
	/// Advertises a service on the local network by means of UDP broadcast.
	/// If you're going to produce broadcast storms using this, I might punch you lovingly.
	/// </summary>
	public class UdpAdvertiser : Service
	{
		public override string Description {
			get {
				return "Advertise string over UDP by listening to broadcast";
			}
		}

		UdpClient udpClient; IPEndPoint anyIP;
		string requestString, responseString;
		byte[] responseBytes;
		int advertisePort;

		protected override void Initialize (Settings modSettings)
		{
			requestString = modSettings.GetString("request", "tost");
			responseString = modSettings.GetString("response", "test");
			advertisePort = int.Parse(modSettings.GetString("port", "15325"));

			responseBytes = Encoding.ASCII.GetBytes(responseString);

			anyIP = new IPEndPoint(IPAddress.Any, advertisePort);
			udpClient = new UdpClient(anyIP);

			BeginAdvertising();
		}

		/// <summary>
		/// Begins the advertising.
		/// </summary>
		private void BeginAdvertising()
		{	
			udpClient.BeginReceive(AdvertiseRoutine, null);
		}

		/// <summary>
		/// Advertising-routine
		/// </summary>
		/// <param name="result">Result.</param>
		private void AdvertiseRoutine (IAsyncResult result)
		{
			byte[] receivedBytes = udpClient.EndReceive (result, ref anyIP);
			string receivedString = Encoding.ASCII.GetString (receivedBytes);

			Secretary.Report(5, "received", receivedString, "from", anyIP.ToString());

			if (receivedString == requestString) {
				udpClient.Send(responseBytes, responseBytes.Length, anyIP);
			}

			BeginAdvertising();
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}


		protected override bool Process (IInteraction parameters)
		{
			return false;
		}
	
	}
}

