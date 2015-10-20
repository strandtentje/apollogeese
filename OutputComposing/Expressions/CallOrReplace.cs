using System;
using System.IO;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	public class CallOrReplace : IExpression
	{
		Call Call {
			get;
			set;
		}

		Replacement Replace {
			get;
			set;
		}

		public CallOrReplace (string rawExpression, Template template)
		{
			this.Call = new Call (rawExpression, template);
			this.Replace = new Replacement (rawExpression);
		}

		public bool TryWriteTo (StreamWriter writer, IInteraction context) {
			return this.Call.TryWriteTo (
				writer, context) || this.Replace.TryWriteTo (
				writer, context);
		}
	}
}

