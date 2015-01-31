using System;
using BorrehSoft.ApolloGeese.Duckling;

namespace BorrehSoft.Extensions.BasicWeblings.Server
{
	
	class UdpQueryResult : QuickInteraction
	{
		public string HostIP { get; private set; }
		public string ResultString { get; private set; }
		public string Combined { get; private set; }
		public DateTime CreationTime { get; private set; }

		public override bool Has (string key)
		{
			return base.Has(key) ;
		}

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
