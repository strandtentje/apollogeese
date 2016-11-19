using System.IO;
using BorrehSoft.Utilities.Collections;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Collections.Specialized;

namespace BorrehSoft.ApolloGeese.CoreTypes
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

		void SetCookie (string name, string value, bool httpOnly = true);

		string GetCookie (string name);

		void SetStatusCode (int statuscode);

		void SetContentType (string contentType);

		void SetContentLength (long contentLength);

		void FlushBuffer();

		void PurgeBuffer();
	}
}

