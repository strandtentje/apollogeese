using System;
using BorrehSoft.Utensils;
using System.IO;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Duckling
{
	/// <summary>
	/// Parameters to an HTTP-related service-interaction.
	/// </summary>
	public interface IHttpInteraction : IInteraction
	{
		/// <summary>
		/// Gets the URL chunk-list
		/// </summary>
		/// <value>
		/// The URL
		/// </value>
		StringList URL { get; }

		/// <summary>
		/// Gets the request headers.
		/// </summary>
		/// <value>The request headers.</value>
		NameValueCollection RequestHeaders { get; }

		/// <summary>
		/// Gets the request cookies.
		/// </summary>
		/// <value>The request cookies.</value>
		CookieCollection RequestCookies { get; }

		/// <summary>
		/// Gets the request  method.
		/// </summary>
		/// <value>The request body method.</value>
		string RequestMethod { get; }

		/// <summary>
		/// Gets the MIME type of the request body.
		/// </summary>
		/// <returns>The body type.</returns>
		string RequestBodyMIME { get; }

		/// <summary>
		/// Gets the size of the request body.
		/// </summary>
		/// <value>The size of the request body.</value>
		byte RequestBodySize { get; }


		/// <summary>
		/// Gets the request body encoding.
		/// </summary>
		/// <value>The request body encoding.</value>
		Encoding RequestBodyEncoding { get; }

		/// <summary>
		/// Gets the request body stream.
		/// </summary>
		/// <value>The request body stream.</value>
		Stream RequestBodyStream { get; }
		
		/// <summary>
		/// Gets or sets the status code for the HTTP response
		/// </summary>
		/// <value>
		/// The status code.
		/// </value>
		int StatusCode { get; set; }

		/// <summary>
		/// Gets the response headers.
		/// </summary>
		/// <value>The response headers.</value>
		NameValueCollection ResponseHeaders { get; }

		/// <summary>
		/// Gets or sets the response cookies.
		/// </summary>
		/// <value>The response cookies.</value>
		CookieCollection ResponseCookies { get; set; }

		/// <summary>
		/// Appends text to response body.
		/// </summary>
		/// <param name='copy'>
		/// Copy to append
		/// </param>
		/// <param name='type'>
		/// Type of copy (MIME!) (i.e. text/html)
		/// </param>
		void AppendToBody (string copy, string type);

		/// <summary>
		/// Sets the response body.
		/// </summary>
		/// <param name='sourceStream'>
		/// Source stream.
		/// </param>
		/// <param name='sourceType'>
		/// Source type (MIME!)
		/// </param>
		/// <param name='sourceLength'>
		/// Source length.
		/// </param>
		void SetBody (Stream sourceStream, string sourceType, long sourceLength);
	}
}

