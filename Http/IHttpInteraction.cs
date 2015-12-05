using System.IO;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Collections.Specialized;

namespace BorrehSoft.ApolloGeese.Http
{
	/// <summary>
	/// Parameters to an HTTP-related service-interaction.
	/// </summary>
	public interface IHttpInteraction : IInteraction, IHeaderedInteraction, IOutgoingBodiedInteraction, IIncomingBodiedInteraction
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

		void SetCookie (string name, string value);

		string GetCookie (string name);

		/// <summary>
		/// Gets or sets the status code for the HTTP response
		/// </summary>
		/// <value>
		/// The status code.
		/// </value>
		void SetStatuscode(int statuscode);
	}
}

