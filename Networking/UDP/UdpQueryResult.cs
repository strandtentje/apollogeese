using System;
using BorrehSoft.ApolloGeese.Duckling;

namespace BorrehSoft.ApolloGeese.Extensions.Networking.UDP
{
	/// <summary>
	/// UDP query result.
	/// </summary>
	class UdpQueryResult : QuickInteraction
	{
		/// <summary>
		/// Gets the host IP
		/// </summary>
		/// <value>The host I.</value>
		public string HostIP { get; private set; }

		/// <summary>
		/// Gets the result string.
		/// </summary>
		/// <value>The result string.</value>
		public string ResultString { get; private set; }

		/// <summary>
		/// Gets the combined host IP and result string
		/// </summary>
		/// <value>The combined.</value>
		public string Combined { get; private set; }

		/// <summary>
		/// Gets the creation time of this query result.
		/// </summary>
		/// <value>The creation time.</value>
		public DateTime CreationTime { get; private set; }

		public override object Get (string key)
		{
			if (key == "hostip") return HostIP;
			if (key == "resultstring") return ResultString;
			if (key == "combined") return Combined;
			if (key == "creationtime") return CreationTime.ToString("o");

			return base.Get (key);
		}

		public UdpQueryResult (string hostIP, string resultString)
		{
			this["hostip"] = ""; this["resultstring"] = ""; this["combined"] = ""; this["creationtime"] = "";

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
}
