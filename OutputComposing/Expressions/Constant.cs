using System;
using System.IO;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	class Constant : NamedExpression
	{
		private string Value;

		public Constant(string value) {
			this.Value = value;
		}

		public override bool TryWriteTo (StreamWriter writer, IInteraction context)
		{
			writer.Write (Value);
			return true;
		}
	}
}

