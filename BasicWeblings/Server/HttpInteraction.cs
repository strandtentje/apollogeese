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

namespace BorrehSoft.Extensions.BasicWeblings.Server
{
	/// <summary>
	/// Http interaction, offers extra utilities for interactions that resulted from an HTTP
	/// server.
	/// </summary>
	public class HttpInteraction : QuickInteraction, IHttpInteraction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Extensions.BasicWeblings.Server.HttpInteraction"/> class.
		/// </summary>
		/// <param name="Request">Request.</param>
		/// <param name="Response">Response.</param>
		public HttpInteraction(HttpListenerRequest Request, HttpListenerResponse Response, bool IsTainted = false)
		{
			this.IsTainted = IsTainted;
			this.Request = Request;
			this.Response = Response;
		}

		public bool IsTainted { get; private set; }

		#region Request
		HttpListenerRequest _request;

		string urlsection, querysection;

		public string GetQuery { get { return querysection; } }

		/// <summary>
		/// Gets the request.
		/// </summary>
		/// <value>The request.</value>
		public HttpListenerRequest Request {
			get { return _request;	}
			private set 
			{ 
				_request = value; 

				this["remoteip"] = _request.RemoteEndPoint.Address.ToString();

				SetUrl(value.RawUrl);

				RequestHeaders = new RequestHeaders (value.Headers, Request.Cookies);
				IncomingBody = new StreamReader (value.InputStream);
			}
		}

		private void SetUrl(string rawurl) {
			string[] parts = rawurl.Split(new char[] { '?' }, StringSplitOptions.RemoveEmptyEntries);
 			this["pathsection"] = urlsection = parts[0].Trim('/');

			if (parts.Length > 1) this["querysection"] = querysection = parts[1];

			this["url"] = URL = new StringList (urlsection, '/');
		}

		/// <summary>
		/// Gets the request method.
		/// </summary>
		/// <value>The request body method.</value>
		public string RequestMethod { get { return Request.HttpMethod; } }

		/// <summary>
		/// Gets the URL chunk list.
		/// </summary>
		/// <value>The URL chunk list</value>
		public StringList URL { get; private set; }

		/// <summary>
		/// Gets the request headers.
		/// </summary>
		/// <value>The request headers.</value>
		public RequestHeaders RequestHeaders { get; private set; }

		/// <summary>
		/// Gets the request body.
		/// </summary>
		/// <value>The request body.</value>
		public StreamReader IncomingBody { get; private set; }
		#endregion

		#region Response		
		HttpListenerResponse _response;

		/// <summary>
		/// Gets or sets the response.
		/// </summary>
		/// <value>The response.</value>
		public HttpListenerResponse Response { 
			get { return _response; }
			set {
				_response = value;
				if (!IsTainted) SetHeadAndBodyFromResponse(value);
			}
		}



		/// <summary>
		/// Gets or sets the status code for the HTTP response
		/// </summary>
		/// <value>The status code.</value>
		public int StatusCode {
			get {
				return Response.StatusCode;
			}
			set {
				Response.StatusCode = value;
			}
		}

		void SetHeadAndBodyFromResponse (HttpListenerResponse value)
		{			
			_responseHeaders = new ResponseHeaders (value.Headers);
			OutgoingBody = new StreamWriter (value.OutputStream);
		}

		ResponseHeaders _responseHeaders;

		/// <summary>
		/// Gets the response headers.
		/// </summary>
		/// <value>The response headers.</value>
		public ResponseHeaders ResponseHeaders { get { return _responseHeaders; } }

		public StreamWriter OutgoingBody { get; private set; }
		#endregion
	
	}
}