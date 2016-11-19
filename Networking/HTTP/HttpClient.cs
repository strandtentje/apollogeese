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

namespace BorrehSoft.ApolloGeese.Extensions.Networking.TCP
{
	/// <summary>
	/// Comatibility Synonym for HTTP.
	/// </summary>
	public class HttpClient : HTTP
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
			Settings ["uri"] = defaultParameter;
		}
    }
}
