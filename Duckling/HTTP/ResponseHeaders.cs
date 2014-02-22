using System;
using BorrehSoft.BorrehSoft.Utensils.Collections;
using System.IO;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.Duckling.Http.Headers;

namespace BorrehSoft.ApolloGeese.Duckling.Http
{
	/// <summary>
	/// Response headers.
	/// </summary>
	public class ResponseHeaders
	{
		public NameValueCollection Backend = new NameValueCollection();

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.Duckling.Http.ResponseHeaders"/> class.
		/// </summary>
		/// <param name="Backend">Backend.</param>
		/// <param name="defaultMime">Default MIME.</param>
		/// <param name="defaultCharset">Default charset.</param>
		public ResponseHeaders(NameValueCollection Backend, string defaultMime = "text/html", string defaultCharset = "utf-8")
		{
			this.Backend = Backend;

			this.Backend ["Content-Type"] = defaultMime + "; charset=" + defaultCharset;
		}

		/// <summary>
		/// Gets or sets the type of the content.
		/// </summary>
		/// <value>The type of the content.</value>
		public MimeType ContentType {
			get {
				string cType = Backend ["Content-Type"];

				if ((cType != null) && (cType.Length > 0))
					return MimeType.FromString (cType);

				return null;
			}
			set {
				Backend ["Content-Type"] = value.ToString ();
			}
		}

		/// <summary>
		/// Gets the length of the content.
		/// </summary>
		/// <value>The length of the content.</value>
		public long ContentLength {
			get {
				long length;
				if (long.TryParse (Backend ["Content-Length"], out length)) {
					return length;
				} else {
					ContentLength = 0;
					return 0;
				}
			}
			set {
				Backend ["Content-Length"] = value.ToString ();
			}
		}

		/// <summary>
		/// Sets a cookie.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="value">Value.</param>
		public void SetCookie(string name, string value)
		{
			Backend.Add("Set-Cookie", string.Format("{0}={1};", name, value));
		}
	}
}

