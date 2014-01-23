using System;
using System.Net;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.Extensions.BasicWeblings
{
	/// <summary>
	/// Http interaction, offers extra utilities for interactions that resulted from an HTTP
	/// server.
	/// </summary>
	public class HttpInteraction : Interaction
	{
		/// <summary>
		/// Gets or sets the request.
		/// </summary>
		/// <value>The request.</value>
		public HttpListenerRequest Request {
			get { return (HttpListenerRequest)this ["Request"];	}
			set { this ["Request"] = value; }
		}

		/// <summary>
		/// Gets or sets the response.
		/// </summary>
		/// <value>The response.</value>
		public HttpListenerResponse Response {
			get { return (HttpListenerResponse)this ["Response"]; }
			set { this ["Response"] = value; }
		}

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
		/// Tries to get a string from the underlying map.
		/// </summary>
		/// <returns><c>true</c>, if get string was found and returned, <c>false</c> otherwise.</returns>
		/// <param name="name">Name of the map entry.</param>
		/// <param name="chunk">Value of the map entry.</param>
		public bool TryGetString (string name, out string chunk)
		{
			if (this [name] == null)
				return false;

			if (this [name] is string) {
				chunk = (string)this [name];
				return true;
			}

			return false;
		}
	}
}

