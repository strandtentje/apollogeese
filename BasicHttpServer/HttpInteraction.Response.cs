using System;
using System.Net;
using System.IO;
using System.Web;
using System.Collections.Specialized;

namespace BorrehSoft.ApolloGeese.Extensions.BasicHttpServer
{
	/// <summary>
	/// Outgoing portion of HTTP interaction.
	/// </summary>
	public partial class HttpInteraction
	{

		Stream streamToClient;
		MemoryStream bufferStream = new MemoryStream();

		HttpListenerResponse _response;

		/// <summary>
		/// Gets or sets the response.
		/// </summary>
		/// <value>The response.</value>
		public HttpListenerResponse Response { 
			get { return _response; }
			set {
				_response = value;
				ResponseHeaders = value.Headers;
				streamToClient = value.OutputStream;
			}
		}

		public void SetStatusCode(int statuscode)
		{
			if (SkipBuffer) {
				throw new HttpException ("Can't change status; already writing data");
			} else {
				Response.StatusCode = statuscode;
			}
		}

		public void SetContentType(string contentType)
		{
			if (SkipBuffer) {
				throw new HttpException ("Can't change mimetype; already writing data");
			} else {
				Response.ContentType = contentType;
			}
		}

		public void SetContentLength(long contentLength) 
		{
			if (SkipBuffer) {
				throw new HttpException ("Can't change contentlength; already writing data");
			} else {
				Response.ContentLength64 = contentLength;
			}
		}

		public void PurgeBuffer()
		{
			SkipBuffer = true;
		}

		public void FlushBuffer()
		{
			if (bufferStream.Position > 0) {
				bufferStream.Position = 0;
				bufferStream.CopyTo (streamToClient);
			}

			SkipBuffer = true;
		}

		public bool SkipBuffer { get; private set; }

		public void SetCookie(string name, string value, bool isSecureSession = true) {
			if (Response.Cookies [name] != null) {
				Response.Cookies [name].Value = value;
			} else {
				Cookie cookie = new Cookie (name, value);
				cookie.HttpOnly = isSecureSession;
				cookie.Secure = isSecureSession;
				Response.SetCookie (cookie);
			}
		}

		/// <summary>
		/// Gets the response headers.
		/// </summary>
		/// <value>The response headers.</value>
		public NameValueCollection ResponseHeaders { get; private set; }

		/// <summary>
		/// Gets the outgoing body.
		/// </summary>
		/// <value>The outgoing body.</value>
		public Stream OutgoingBody { 
			get {
				if (SkipBuffer) {
					return streamToClient;
				} else {
					return bufferStream;
				}
			}
		}
	}
}

