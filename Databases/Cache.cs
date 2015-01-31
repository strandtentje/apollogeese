using System;
using BorrehSoft.ApolloGeese.Duckling;

namespace BorrehSoft.ApolloGeese.Extensions.Data
{
	public abstract class Cache : Service
	{
		public Cache ()
		{
		}

		public override string Description {
			get {
				return "Caches HttpResponses";
			}
		}
	}
}

