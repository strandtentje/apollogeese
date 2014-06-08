using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.ApolloGeese.Duckling.Http;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class IterableInteraction : IInteraction, Map<object>
	{
		public IterableInteraction (IHttpInteraction Parent)
		{
			this.Parent = Parent;
		}

		public IInteraction Parent {
			get;
			private set;
		}

		public IInteraction Root {
			get { return this.Parent.Root; }
		}		

		public IHttpInteraction Sink {
			get;
			set;
		}

		public Func<IInteraction, bool> ResultCallback {
			get;
			set;
		}
	}
}

