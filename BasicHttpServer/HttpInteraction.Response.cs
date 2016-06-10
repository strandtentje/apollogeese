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

		/// <summary>
		/// Sets the HTTP-statuscode, once.
		/// </summary>
		/// <value>The status code.</value>
		/// <param name="statuscode">Statuscode.</param>
		public void SetStatuscode(int statuscode) {
			if (IsStatuscodeSet) {
				throw new HttpException ("Statuscode can only be set once");
			} else {
				Response.StatusCode = statuscode;

				if (bufferStream.Position > 0) {
					bufferStream.Position = 0;
					bufferStream.CopyTo (streamToClient);
				}

				IsStatuscodeSet = true;
			}
		}

		public bool IsStatuscodeSet { get; private set; }

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
				if (IsStatuscodeSet) {
					return streamToClient;
				} else {
					return bufferStream;
				}
			}
		}
	}
}

