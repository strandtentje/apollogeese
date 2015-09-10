using System;
using System.Collections.Generic;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Cache
{
	class CacheException : Exception
	{
		public CacheException (string message) : base(message)
		{

		}
	}
}

