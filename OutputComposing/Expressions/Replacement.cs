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
	class Replacement : NamedExpression
	{
		public Replacement (string rawExpression)
		{
			this.Expression = rawExpression;
		}

		public override bool TryWriteTo (StreamWriter writer, IInteraction context) {
			string value;
			if (context.TryGetFallbackString (this.Name, out value)) {
				writer.Write (value);
				return true;
			}
			return false;
		}
	}
}

