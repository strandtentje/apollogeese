using System;

namespace BorrehSoft.ApolloGeese.Duckling
{
	public interface ISessionInteraction 
	{
		/// <summary>
		/// Gets or sets the user token.
		/// </summary>
		/// <value>The user token.</value>
		string UserToken { get; }

		/// <summary>
		/// Gets or sets the session cookie.
		/// </summary>
		/// <value>The session cookie.</value>
		string SessionCookie { get; }
	}
}

