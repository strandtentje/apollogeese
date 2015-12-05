using System;
using System.Collections.Specialized;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public interface IHeaderedInteraction
	{

		/// <summary>
		/// Gets the request headers.
		/// </summary>
		/// <value>The request headers.</value>
		NameValueCollection RequestHeaders { get; }

		/// <summary>
		/// Gets the response headers.
		/// </summary>
		/// <value>The response headers.</value>
		NameValueCollection ResponseHeaders { get; }

	}
}

