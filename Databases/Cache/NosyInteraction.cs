using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Text;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Cache
{
	class NosyInteraction : SimpleInteraction, INosyInteraction
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
