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
		public HttpListenerRequest Incoming { get; private set; }
		
		/// <summary>
		/// Gets or sets the outgoing cookies.
		/// </summary>
		/// <value>The out cookies.</value>
		public CookieCollection OutCookies { get; set; }

		/// <summary>
		/// Gets or sets the response encoding.
		/// </summary>
		/// <value>The response encoding.</value>
		public Encoding ResponseEncoding { get; set; }

		/// <summary>
		/// Gets the URL queue.
		/// </summary>
		/// <value>The URL queue.</value>
		public Queue<string> UrlQueue { get; set; }

		/// <summary>
		/// Gets or sets the HTTP status code.
		/// </summary>
		/// <value>The status code.</value>
		public int StatusCode {	get; set; }

		/// <summary>
		/// Gets or sets the HTML code.
		/// </summary>
		/// <value>The HTM.</value>
		public StringBuilder HTML { get; set; }

		/// <summary>
		/// Gets or sets the Mime Type of the response.
		/// </summary>
		/// <value>The type of the MIME. (What the hell Monodevelop Autogenerate?)</value>
		public string MimeType { get; set; }

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

		/// <summary>
		/// Gets or sets the outgoing headers.
		/// </summary>
		/// <value>The outgoing headers.</value>
		public WebHeaderCollection OutgoingHeaders { get; set;	}

		/// <summary>
		/// Gets or sets the luggage that is produced and consumed when
		/// processing the request
		/// </summary>
		/// <value>The luggage.</value>
		public Map<object> Luggage { get; set; }
				
		/// <summary>
		/// Gets the POST message body if any.
		/// </summary>
		/// <value>The message body.</value>
		public Map<string> MessageBody { get; private set; }

		private Interaction () { }

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.Duckling.Parameters"/> class.
		/// </summary>
		/// <param name="request">Request to parameterize.</param>
		public Interaction (HttpListenerRequest request)
		{
			Incoming = request;

			string[] tailList = HttpUtility.UrlDecode (Incoming.RawUrl).Trim ('/').Split ('/');
			UrlQueue = new Queue<string> (tailList);

			StatusCode = 200;
			HTML = new StringBuilder ();
			MimeType = "text/html";
			OutgoingHeaders = new WebHeaderCollection ();
			Luggage = new Map<object> ();
			OutCookies = new CookieCollection ();	
		}

		/// <summary>
		/// Clone this instance into a clear set of Parameters
		/// </summary>
		public Interaction Clone()
		{
			return new Interaction () {
				Incoming = this.Incoming,
				UrlQueue = new Queue<string>(this.UrlQueue.ToArray()),
				StatusCode = this.StatusCode,
				HTML = new StringBuilder(),
				MimeType = this.MimeType,
				ResponseEncoding = this.ResponseEncoding,
				Luggage = this.Luggage.Clone(),
				OutCookies = this.OutCookies
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

