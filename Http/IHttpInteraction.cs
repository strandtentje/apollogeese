using System.IO;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.ApolloGeese.Duckling;

namespace BorrehSoft.ApolloGeese.Http
{
	/// <summary>
	/// Parameters to an HTTP-related service-interaction.
	/// </summary>
	public interface IHttpInteraction : IInteraction, IOutgoingBodiedInteraction, IIncomingBodiedInteraction
	{
		/// <summary>
		/// Gets the request  method.
		/// </summary>
		/// <value>The request body method.</value>
		string RequestMethod { get; }

		/// <summary>
		/// Gets the URL chunk-list
		/// </summary>
		/// <value>
		/// The URL
		/// </value>
		StringList URL { get; }

		string GetQuery { get; }

		/// <summary>
		/// Gets the request headers.
		/// </summary>
		/// <value>The request headers.</value>
		RequestHeaders RequestHeaders { get; }

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
		ResponseHeaders ResponseHeaders { get; }

	}
}

