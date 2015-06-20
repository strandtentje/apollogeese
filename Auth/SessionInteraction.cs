using System;
using System.Collections.Generic;
using URandom = BorrehSoft.Utensils.Random;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.ApolloGeese.Http;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.Auth
{
	class SessionInteraction : QuickInteraction
	{
		public SessionInteraction (IInteraction uncastParameters, string cookieName, string givenCookie) : base(uncastParameters)
		{
			this [cookieName] = givenCookie;
		}
	}
}

