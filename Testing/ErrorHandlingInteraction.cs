using System;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace Testing
{
	public class ErrorHandlingInteraction : BareInteraction
	{		
		public ErrorHandlingInteraction(
			IInteraction parameters, ExceptionHandler handler) : base(
				parameters)
		{
			this.ExceptionHandler = handler;
		}

		public override IInteraction Clone (IInteraction parent)
		{
			return new ErrorHandlingInteraction (parent, this.ExceptionHandler);
		}
	}
}

