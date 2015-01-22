using System;
using System.IO;
using System.Net;
using System.Web;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils;
using BorrehSoft.Utensils.Log;
using BorrehSoft.ApolloGeese.Duckling.Http;
using System.Collections.Specialized;
using BorrehSoft.Utensils.Collections;
using System.Collections.Generic;
using System.Text;

namespace BorrehSoft.Extensions.BasicWeblings.Server
{
	/// <summary>
	/// Http interaction, offers extra utilities for interactions that resulted from an HTTP
	/// server.
	/// </summary>
	public partial class HttpInteraction : QuickInteraction, IHttpInteraction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Extensions.BasicWeblings.Server.HttpInteraction"/> class.
		/// </summary>
		/// <param name="Request">Request.</param>
		/// <param name="Response">Response.</param>
		public HttpInteraction(HttpListenerRequest Request, HttpListenerResponse Response)
		{
			this.Request = Request;
			this.Response = Response;
		}	
	}
}