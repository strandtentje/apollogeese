using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.Text;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Cache
{
	class NosyInteraction : QuickInteraction, INosyInteraction
	{
		private bool watchContext;

		public NosyInteraction(bool watchContext, IInteraction parent) : base(parent) {
			this.watchContext = watchContext;
		}

		public bool IncludeContext {
			get {
				return this.watchContext;
			}
		}

		public string Signature {get;set;}
	}
}
