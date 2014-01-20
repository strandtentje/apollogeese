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
using GList = System.Collections.Generic.List<string>;

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
		private GList _urlAhead;
		private GList _urlProcessed = new GList();

		private Map<string> _messageBody;

		/// <summary>
		/// Gets the URL chunks that haven't been processed.
		/// </summary>
		/// <value>The unprocessed chunks of the url.</value>
		public string[] UrlAhead { 
			get {
				if (_urlAhead != null)
					return _urlAhead.ToArray ();
								
				string[] tailList = HttpUtility.UrlDecode (Incoming.RawUrl).Trim ('/').Split ('/');

				_urlAhead = new GList (tailList);

				return tailList;
			}
		}

		/// <summary>
		/// Gets the URL, processed thus far.
		/// </summary>
		/// <value>The URL that has yet to be processed.</value>
		public string[] UrlProcessed {
			get {
				return _urlProcessed.ToArray ();
			}
		}

		/// <summary>
		/// Gets the next URL chunk.
		/// </summary>
		/// <returns>The next URL chunk.</returns>
		public bool InvokeForNextURLChunk(Action<string> invokeAction)
		{
			if (_urlAhead.Count < 1) return false;

			string chunk = _urlAhead [0]; 

			_urlAhead.RemoveAt (0);
			_urlProcessed.Insert (0, chunk);

			invokeAction (chunk);

			_urlProcessed.RemoveAt (0);
			_urlAhead.Insert (0, chunk);

			return true;
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
					_messageBody = new Map<string>();
					HttpInterations.ReadIntoMap(Incoming.InputStream, '=', '&', ref _messageBody);
				}

				return _messageBody;
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
		public long Size;

		/// <summary>
		/// Gets or sets the stream to use for the data in the response.
		/// </summary>
		/// <value>The body stream.</value>
		public Stream BodyInStream;
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
			Interaction clonedInteraction = new Interaction (this.Incoming, this.maxInLen) {
				_urlAhead = new GList(this._urlAhead.ToArray()),
				_urlProcessed = new GList(this._urlProcessed.ToArray()),
				StatusCode = this.StatusCode,
				MimeType = this.MimeType,
				ResponseEncoding = this.ResponseEncoding,
				Luggage = this.Luggage.Clone(),
				OutCookies = this.OutCookies
			};

			return clonedInteraction;
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

