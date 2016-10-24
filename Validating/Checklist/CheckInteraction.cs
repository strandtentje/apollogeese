using System;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace Validating
{
	public class CheckInteraction : BareInteraction
	{
		public CheckInteraction (IInteraction parent) : base(parent)
		{
			this.Successful = true;
			this.ExceptionHandler = parent.ExceptionHandler;
		}	

		public bool Successful { get; set; }

		public override IInteraction Clone (IInteraction parent)
		{
			throw new UnclonableException ();
		}
	}
}

