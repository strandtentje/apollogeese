using System;
using System.Net;
using BorrehSoft.ApolloGeese.Http;
using System.Collections.Generic;
using System.Text;
using BorrehSoft.Utensils.Collections;
using System.IO;
using System.Collections.Specialized;

namespace BorrehSoft.ApolloGeese.Extensions.BasicHttpServer
{
	/// <summary>
	/// Request portion of HTTP interaction
	/// </summary>
	public partial class HttpInteraction
	{
		HttpListenerRequest _request;

		string urlsection, querysection;

		/// <summary>
		/// Returns the URL-query (that is the query after the ? in the url)
		/// </summary>
		/// <value>The GET query.</value>
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

				RequestHeaders = Request.Headers;
				IncomingBody = value.InputStream;
			}
		}

		public string GetCookie(string name) {
			return Request.Cookies [name].Value;

		}

		/// <summary>
		/// Sets the URL that invoked this HTTP-interaction
		/// </summary>
		/// <param name="rawurl">Raw, textual URL string</param>
		private void SetUrl(string rawurl) {
			Queue<string> parts = new Queue<string> (rawurl.Split ("?".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
			StringBuilder queryBuilder = new StringBuilder ();

			this ["pathsection"] = urlsection = parts.Dequeue ().Trim ('/');

			if (parts.Count > 0) queryBuilder.Append (parts.Dequeue ());

			while (parts.Count > 0) {
				queryBuilder.Append ('?');
				queryBuilder.Append (parts.Dequeue ());
			}

			this ["querysection"] = querysection = queryBuilder.ToString ();

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
		public NameValueCollection RequestHeaders { get; private set; }

		/// <summary>
		/// Gets the request body.
		/// </summary>
		/// <value>The request body.</value>
		public Stream IncomingBody { get; private set; }

		private StreamReader reader = null;

		/// <summary>
		/// Gets a value indicating whether or not a reader has been produced for the stream
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public bool HasReader() {
			return reader != null;
		}

		/// <summary>
		/// Gets the incoming body reader.
		/// </summary>
		/// <returns>The incoming body reader.</returns>
		public TextReader GetIncomingBodyReader() {
			if (reader == null)
				reader = new StreamReader (IncomingBody);

			return reader;
		}
	}
}

