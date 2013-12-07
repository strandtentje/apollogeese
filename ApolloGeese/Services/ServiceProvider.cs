using System;
using System.Net;

namespace ApolloGeese.Services
{
	/// <summary>
	/// Service provider.
	/// </summary>
	public abstract class ServiceProvider
	{
		/// <summary>
		/// Gets the service to be provided.
		/// </summary>
		/// <value>
		/// The service.
		/// </value>
		public abstract Service Service { get; }

		/// <summary>
		/// Detect if a request is of interrest for us.
		/// </summary>
		/// <param name='request'>
		/// Returns true if the request is of interrest for us.
		/// </param>
		public abstract bool Detect(HttpListenerRequest request);

		/// <summary>
		/// Parse the specified request into parameters the 
		/// Service will understand.
		/// </summary>
		/// <param name='request'>
		/// Request.
		/// </param>
		public abstract ServiceParams Parse(HttpListenerRequest request);
	}
}

