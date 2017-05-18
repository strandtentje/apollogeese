using System;
using System.Collections.Generic;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Auth
{
	class SessionException : Exception
	{
		public SessionException (string message) : base(message)
		{

		}
	}
}

