using System;
using System.IO;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections;

namespace BorrehSoft.ApolloGeese.Http
{
	public class RequestHeaders
	{
		public NameValueCollection Backend;
		public Map<string> Cookies = new Map<string>();

		public RequestHeaders (NameValueCollection Backend)
		{
			this.Backend = Backend;

			// SetCookies (Backend.GetValues ("Cookie"));
		}

		public RequestHeaders (NameValueCollection headers, CookieCollection cookiesFromRequest)
		{
			this.Backend = headers;
			foreach (Cookie cookie in cookiesFromRequest) 
				this.Cookies[cookie.Name] = cookie.Value;
		}
	}
}

