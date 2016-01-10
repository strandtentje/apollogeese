using System;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace Validating
{
	public class CheckInteraction : BareInteraction
	{
		public CheckInteraction (IInteraction parent) : base(parent)
		{
			this.Successful = true;
		}	

		public bool Successful { get; set; }
	}
}

