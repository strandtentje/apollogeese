using System;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	class VariableCall : NamedExpression
	{
		Template Caller;
		Map<Service> Branches;
		const string TEMPLATE_VARIABLE = "templatevariable";

		public VariableCall (string rawExpression, Template template)
		{
			this.Expression = rawExpression;
			this.Caller = template;
			this.Branches = this.Caller.Branches;
		}

		public override bool TryWriteTo (StreamWriter writer, IInteraction context) {
			Service called = null;
			IInteraction callContext;

			if (Branches.Has (TEMPLATE_VARIABLE)) {
				called = Branches [TEMPLATE_VARIABLE];
				callContext = new SimpleInteraction (context, TEMPLATE_VARIABLE, this.Name);
			} else {
				return false;
			}

			return called.TryProcess (callContext);
		}
	}
}

