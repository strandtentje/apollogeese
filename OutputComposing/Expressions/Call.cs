using System;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using System.Net;
using System.IO;
using BorrehSoft.Utilities.Collections;
using Stringtionary = System.Collections.Generic.Dictionary<string, string>;
using System.Text;
using BorrehSoft.Utilities.Log;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	class Call : NamedExpression
	{
		Template Caller;
		Map<Service> Branches;

		public Call (string rawExpression, Template template)
		{
			this.Expression = rawExpression;
			this.Caller = template;
			this.Branches = this.Caller.Branches;
		}
		
		public override bool TryWriteTo (StreamWriter writer, IInteraction context) {
			Service called = null;
			IInteraction callContext;

			if (Branches.Has (this.Name)) {
				called = Branches [this.Name];
				callContext = context;
			} else {
				return false;
			}

			return called.TryProcess (callContext);
		}
	}
}

