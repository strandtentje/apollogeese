using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using System.Net.Sockets;
using System.Net;
using System.IO;
using BorrehSoft.Utilities.Collections;
using BorrehSoft.Utilities.Log;
using System.Text.RegularExpressions;
using System.Text;
using Networking;
using HTTPHTTP = Networking.HTTP;

namespace BorrehSoft.ApolloGeese.Extensions.Networking
{
	/// <summary>
	/// Comatibility Synonym for HTTP.
	/// </summary>
	public class HttpClient : HTTPHTTP
	{
		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			base.HandleBranchChanged (sender, e);

			if (e.Name == "response")
				this.Successful = e.NewValue;
			if (e.Name == "postbuilder")
				this.Body = e.NewValue;
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			string[] requestStart = defaultParameter.Split(' ');

			if (requestStart.Length == 2) {
				Settings["method"] = requestStart[0];
				Settings["uri"] = requestStart[1];
			} else if (requestStart.Length == 1) {
				Settings["uri"] = requestStart[0];
			} else {
				throw new Exception("Request as ([METHOD] )[URL]");
			}
		}
    }
}
