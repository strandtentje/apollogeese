using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.ApolloGeese.Http;
using BorrehSoft.Utensils;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.ApolloGeese.Extensions.BasicHttpServer
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

		public string SourceName {
			get {
				return "http-request-body";
			}
		}
	}
}