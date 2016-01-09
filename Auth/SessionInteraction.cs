using System;
using System.Collections.Generic;
using URandom = BorrehSoft.Utensils.Random;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.Auth
{
	class SessionInteraction : SimpleInteraction
	{
		public SessionInteraction (IInteraction uncastParameters, string cookieName, string givenCookie) : base(uncastParameters)
		{
			this [cookieName] = givenCookie;
		}
	}
}

