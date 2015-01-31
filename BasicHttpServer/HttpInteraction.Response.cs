using System;
using System.Net;
using BorrehSoft.ApolloGeese.Duckling.Http;
using System.IO;

namespace BorrehSoft.ApolloGeese.Extensions.BasicHttpServer
{
	/// <summary>
	/// Outgoing portion of HTTP interaction.
	/// </summary>
	public partial class HttpInteraction
	{
		HttpListenerResponse _response;

		/// <summary>
		/// Gets or sets the response.
		/// </summary>
		/// <value>The response.</value>
		public HttpListenerResponse Response { 
			get { return _response; }
			set {
				_response = value;
				_responseHeaders = new ResponseHeaders (value.Headers);
				OutgoingBody = value.OutputStream;
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

		ResponseHeaders _responseHeaders;

		/// <summary>
		/// Gets the response headers.
		/// </summary>
		/// <value>The response headers.</value>
		public ResponseHeaders ResponseHeaders { get { return _responseHeaders; } }

		/// <summary>
		/// Gets the outgoing body.
		/// </summary>
		/// <value>The outgoing body.</value>
		public Stream OutgoingBody { get; private set; }

		private StreamWriter writer = null;

		/// <summary>
		/// Gets a value indicating whether or not a writer has been produced for the stream
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public bool HasWriter() {
			return writer != null;
		}

		/// <summary>
		/// Gets the outgoing body writer.
		/// </summary>
		/// <returns>The outgoing body writer.</returns>
		public StreamWriter GetOutgoingBodyWriter() {
			if (writer == null)
				writer = new StreamWriter(OutgoingBody);

			return writer;
		}
	}
}

