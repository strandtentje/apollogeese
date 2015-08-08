using System;
using BorrehSoft.ApolloGeese.CoreTypes;

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
		public string HostIP { get { return this.GetString ("hostip"); } }

		/// <summary>
		/// Gets the result string.
		/// </summary>
		/// <value>The result string.</value>
		public string ResultString { get { return this.GetString ("resultstring"); } }

		/// <summary>
		/// Gets the combined host IP and result string
		/// </summary>
		/// <value>The combined.</value>
		public string Combined { get { return this.GetString ("combined"); } }

		/// <summary>
		/// Gets the creation time of this query result.
		/// </summary>
		/// <value>The creation time.</value>
		public DateTime CreationTime { get { return (DateTime)this["creationtime"]; } }

		public UdpQueryResult (string hostIP, string resultString)
		{
			this ["hostip"] = hostIP; 
			this ["resultstring"] = resultString; 
			this ["combined"] = string.Format("{0}{1}", hostIP, resultString); 
			this ["creationtime"] = DateTime.Now;
		}

		public void Update ()
		{
			this["creationtime"] = DateTime.Now;
		}

		public bool IsExpired (TimeSpan maxAge)
		{
			return (DateTime.Now - CreationTime) > maxAge;
		}
	}
}
