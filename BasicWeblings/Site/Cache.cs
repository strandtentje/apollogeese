using System;
using BorrehSoft.ApolloGeese.Duckling;

namespace BorrehSoft.Extensions.BasicWeblings
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

