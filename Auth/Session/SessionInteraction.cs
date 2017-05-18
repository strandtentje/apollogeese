using System;
using System.Collections.Generic;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Auth
{
	class SessionInteraction : SimpleInteraction
	{
		public SessionInteraction (IInteraction uncastParameters, string cookieName, string givenCookie) : base(uncastParameters)
		{
			this [cookieName] = givenCookie;
		}
	}
}

