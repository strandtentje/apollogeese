using System;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using System.Net;
using System.IO;
using BorrehSoft.Utensils.Collections;
using Stringtionary = System.Collections.Generic.Dictionary<string, string>;
using System.Text;
using BorrehSoft.ApolloGeese.Http;
using BorrehSoft.ApolloGeese.Http.Headers;
using BorrehSoft.Utensils.Log;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	class Call : NamedExpression
	{
		Template Caller;

		public Call (string rawExpression, Template template)
		{
			this.Expression = rawExpression;
			this.Caller = template;
		}
		
		public override bool TryWriteTo (StreamWriter writer, IInteraction context) {
			// they should've never told me about branch prediction
			return this.Caller.Branches.Has (
				this.Name) && this.Caller.Branches.Get (
					this.Name).TryProcess (
						context);
		}
	}
}

