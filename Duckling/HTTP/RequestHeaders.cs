using System;
using BorrehSoft.Utensils;
using System.IO;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Duckling.Http
{
	public class RequestHeaders
	{
		public NameValueCollection Backend;
		public Map<string> Cookies;

		public RequestHeaders (NameValueCollection Backend)
		{
			this.Backend = Backend;

			this.Cookies = GetParsedCookies (Backend.GetValues ("Cookie"));
		}

		static Dictionary<string, string> GetParsedCookies (string[] cookieHeaders)
		{
			string[] splitCookie;

			foreach (string cookieSpec in string.Join (";", cookieHeaders)) {
				foreach (string cookiePair in cookieSpec.Split(';')) {
					splitCookie = cookiePair.Split ('=');
					Cookies[splitCookie [0], splitCookie [1]];
				}
			}
		}
	}
}

