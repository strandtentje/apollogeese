using System;
using System.Web;
using System.Net;
using System.Collections.Generic;
using BorrehSoft.Utensils.Settings;
using BorrehSoft.Utensils;
using System.Text;
using System.IO;
using System.Collections.Specialized;
using System.Security;
using System.Security.Cryptography;

namespace BorrehSoft.ApolloGeese.Duckling
{
	/// <summary>
	/// Parameters to a service call
	/// </summary>
	public class Interaction
	{		
		public Map<object> Luggage = new Map<object>();
		private int maxInLen = 10000;

		#region Request
		public readonly HttpListenerRequest Incoming;
		private Queue<string> _urlQueue;
		private Map<object> _messageBody;

		/// <summary>
		/// Gets the URL queue.
		/// </summary>
		/// <value>The URL queue.</value>
		public Queue<string> UrlQueue { 
			get {
				if (_urlQueue != null) return _urlQueue;
								
				string[] tailList = HttpUtility.UrlDecode (Incoming.RawUrl).Trim ('/').Split ('/');
				_urlQueue = new Queue<string> (tailList);

				return _urlQueue;
			}
		}

		/// <summary>
		/// Gets the POST message body if any.
		/// </summary>
		/// <value>The message body.</value>
		public Map<string> MessageBody { 
			get {
				if (_messageBody != null) return _messageBody;

				if (Incoming.ContentLength64 < maxInLen)
				{
					_messageBody = new Map<object>();
					HttpInterations.ReadIntoMap(Incoming.InputStream, '=', '&', _messageBody);
				}
			}
		}
		#endregion

		#region Reponse Headers
		/// <summary>
		/// The status code.
		/// </summary>
		public int StatusCode = 200;

		/// <summary>
		/// The outgoing headers.
		/// </summary>
		public WebHeaderCollection OutgoingHeaders = new WebHeaderCollection();

		/// <summary>
		/// The outgoing cookies.
		/// </summary>
		public CookieCollection OutCookies = new CookieCollection();
		#endregion

		#region Response Body
		/// <summary>
		/// Gets or sets the response encoding.
		/// </summary>
		/// <value>The response encoding.</value>
		public Encoding ResponseEncoding;

		/// <summary>
		/// The HTML source string builder
		/// </summary>
		public StringBuilder HTML = new StringBuilder();

		/// <summary>
		/// The Mimetype to the body of the response
		/// </summary>
		public string MimeType = "text/html";

		/// <summary>
		/// Gets or sets the size of the response in bytes
		/// </summary>
		/// <value>The size.</value>
		public long Size { get; set; }

		/// <summary>
		/// Gets or sets the stream to use for the data in the response.
		/// </summary>
		/// <value>The body stream.</value>
		public Stream BodyInStream { get; set; }
		#endregion

		private Interaction () { }

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.Duckling.Parameters"/> class.
		/// </summary>
		/// <param name="request">Request to parameterize.</param>
		public Interaction (HttpListenerRequest request, int maxInLen = 10000)
		{
			this.Incoming = request;
			this.maxInLen = maxInLen;
		}

		/// <summary>
		/// Clone this instance into a clear set of Parameters
		/// </summary>
		public Interaction Clone()
		{
			return new Interaction () {
				maxInLen = this.maxInLen,
				Incoming = this.Incoming,
				UrlQueue = new Queue<string>(this.UrlQueue.ToArray()),
				StatusCode = this.StatusCode,
				MimeType = this.MimeType,
				ResponseEncoding = this.ResponseEncoding,
				Luggage = this.Luggage.Clone(),
				OutCookies.Add(this.OutCookies)
			};
		}

		/// <summary>
		/// Finish the specified response with these parameters.
		/// </summary>
		/// <param name="response">Response.</param>
		public void Ready (HttpListenerResponse response)
		{
			response.StatusCode = StatusCode;
			response.Headers = OutgoingHeaders;
			response.Cookies = OutCookies;

			if (BodyInStream == null) {
				byte[] buffer = ResponseEncoding.GetBytes (HTML.ToString ());

				response.ContentEncoding = ResponseEncoding;
				response.ContentLength64 = buffer.Length;
				response.ContentType = MimeType;

				response.OutputStream.Write (buffer, 0, buffer.Length);
			} else {
				response.ContentLength64 = Size;
				response.ContentType = MimeType;

				BodyInStream.CopyTo (response.OutputStream);
			}
		}
				
		/// <summary>
		/// Mimics the alternative body stream of another set of parameters.
		/// </summary>
		/// <returns><c>true</c>, if alternative body was set, <c>false</c> otherwise.</returns>
		/// <param name="safeParameters">Safe parameters.</param>
		public bool SetAlternativeBody (Interaction safeParameters)
		{
			if (safeParameters.BodyInStream != null) {
				this.BodyInStream = safeParameters.BodyInStream;
				this.Size = safeParameters.Size;
				this.MimeType = safeParameters.MimeType;
				return true;
			}
			return false;
		}

	}
}

