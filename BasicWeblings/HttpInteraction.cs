using System;
using System.Web;
using System.Net;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils;
using System.IO;

namespace BorrehSoft.Extensions.BasicWeblings
{
	/// <summary>
	/// Http interaction, offers extra utilities for interactions that resulted from an HTTP
	/// server.
	/// </summary>
	public class HttpInteraction : Map<string>, IHttpInteraction
	{
		private HttpListenerRequest _request;
		private HttpListenerResponse _response;

		/// <summary>
		/// Gets or sets the status code for the HTTP response
		/// </summary>
		/// <value>The status code.</value>
		public int StatusCode { get; set; }

		/// <summary>
		/// Gets or sets the request.
		/// </summary>
		/// <value>The request.</value>
		public HttpListenerRequest Request {
			get { return _request;	}
			set 
			{ 
				_request = value; 
				URL = new StringList (value.RawUrl, '/', HttpUtility.UrlDecode);
			}
		}

		/// <summary>
		/// Gets or sets the response.
		/// </summary>
		/// <value>The response.</value>
		public HttpListenerResponse Response {
			get { return _response; }
			set { _response = value; }
		}
		
		/// <summary>
		/// Gets the URL chunk list.
		/// </summary>
		/// <value>The URL chunk list</value>
		public StringList URL { get; private set; }

		/// <summary>
		///  Gets or sets the title of the current block 
		/// </summary>
		/// <value>
		///  The current title. 
		/// </value>
		public string CurrentTitle { get; set; }

		/// <summary>
		/// Appends textual string to body.
		/// </summary>
		/// <param name="str">String to append</param>
		/// <param name="type">MIME-type of string</param>
		public void AppendToBody (string str, string type = "text/html")
		{
			if (Response.ContentEncoding == null)
				Response.ContentEncoding = Request.ContentEncoding;

			if (Response.ContentType != type) {
				if (Response.ContentType.Length > 0) {
					Secretary.Report (5, "Warning: appending '", type, "' to a body of '", Response.ContentType, "'!");
				}
				Response.ContentType = type;

			}

			byte[] bodyBytes = Response.ContentEncoding.GetBytes (str);

			Response.OutputStream.Write (bodyBytes, 0, bodyBytes.Length);
			Response.ContentLength64 += bodyBytes.Length;
		}

		/// <summary>
		/// Sets the body.
		/// </summary>
		/// <param name='sourceStream'>
		/// Source stream.
		/// </param>
		/// <param name='sourceType'>
		/// Source type.
		/// </param>
		/// <param name='sourceLength'>
		/// Source length.
		/// </param>
		public virtual void SetBody (Stream sourceStream, string sourceType, long sourceLength)
		{
			Response.ContentType = sourceType;
			Response.ContentLength64 = sourceLength;
			Response.OutputStream = sourceStream;
		}
	
	}
}