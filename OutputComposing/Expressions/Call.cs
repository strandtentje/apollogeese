using System;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using System.Net;
using System.IO;
using BorrehSoft.Utensils.Collections;
using Stringtionary = System.Collections.Generic.Dictionary<string, string>;
using System.Text;
using BorrehSoft.Utensils.Log;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	class Call : NamedExpression
	{
		Template Caller;
		Map<Service> Branches;
		const string TEMPLATE_VARIABLE = "templatevariable";

		public Call (string rawExpression, Template template)
		{
			this.Expression = rawExpression;
			this.Caller = template;
			this.Branches = this.Caller.Branches;
		}
		
		public override bool TryWriteTo (StreamWriter writer, IInteraction context) {
			// they should've never told me about branch prediction

			Service called = null;
			IInteraction callContext;

			if (Branches.Has (this.Name)) {
				called = Branches [this.Name];
				callContext = context;
			} else if (Branches.Has (TEMPLATE_VARIABLE)) {
				called = Branches [TEMPLATE_VARIABLE];
				callContext = new SimpleInteraction (context, TEMPLATE_VARIABLE, this.Name);
			} else {
				return false;
			}

			return called.TryProcess (callContext);
		}
	}
}

