using System;
using System.Net;
using BorrehSoft.ApolloGeese.Duckling.Http;
using System.Collections.Generic;
using System.Text;
using BorrehSoft.Utensils.Collections;
using System.IO;

namespace BorrehSoft.Extensions.BasicWeblings.Server
{
	public partial class HttpInteraction
	{
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
				IncomingBody = value.InputStream;
			}
		}

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
		public RequestHeaders RequestHeaders { get; private set; }

		/// <summary>
		/// Gets the request body.
		/// </summary>
		/// <value>The request body.</value>
		public Stream IncomingBody { get; private set; }

		private StreamReader reader = null;

		public bool HasReader() {
			return reader != null;
		}

		public StreamReader GetIncomingBodyReader() {
			if (reader == null)
				reader = new StreamReader (IncomingBody);

			return reader;
		}
	}
}

